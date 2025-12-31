using System;
using Core;
using Cysharp.Threading.Tasks;
using Defines;
using Dialog;
using Effect;
using Engine.ShelfPuzzle;
using Factory;
using Game;
using JetBrains.Annotations;
using manager;
using manager.Interface;
using Senspark;
using Strategy.Level;
using UnityEngine;
using Utilities;

namespace UI
{
    [SceneMusic(AudioEnum.GameMusic)]
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] private GameObject container;
        [SerializeField] private ShelfItemBasic shelfItemPrefab;
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private LevelUI levelUI;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private ILevelLoaderManager _levelLoaderManager;
        private ILevelManager _levelManager;
        private IAudioManager _audioManager;

        private GameStateType State { get; set; } = GameStateType.UnInitialize;
        private LevelView _levelView;
        private bool _didDrag = false;

        private void Awake()
        {
            try
            {
                GetServices();
            }
            catch (Exception e)
            {
                // missing service - re-initialize
                UniTaskExtensions.Forget(MockGameInitializer.Initialize()
                        .ContinueWith(() =>
                        {
                            GetServices();
                            Start();
                        }));
            }

            return;

            void GetServices()
            {
                var services = ServiceLocator.Instance;
                _levelLoaderManager = services.Resolve<ILevelLoaderManager>();
                _levelManager = services.Resolve<ILevelManager>();
                _audioManager = services.Resolve<IAudioManager>();
                State = GameStateType.Initialized;
            }
        }

        private void Start()
        {
            if (State != GameStateType.Initialized) return;
            Time.timeScale = 1;
            dragDropManager.Init(CanAcceptDropInto);
            CleanUp();
            LoadLevel(_levelManager.GetCurrentLevel());
            InvokeRepeating(nameof(IntervalAutoCheckDeadlock), 5f, 3f);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _levelAnimation?.Update(dt);
            ProcessUpdate();
        }

        private void ProcessUpdate()
        {
            if (State == GameStateType.Paused) return;
            if (!_didDrag && dragDropManager.IsDragging())
            {
                if (State < GameStateType.Playing)
                {
                    _didDrag = true;
                    State = GameStateType.Playing;
                }
            }
            if (State == GameStateType.Playing)
            {
                _levelView.Step(Time.deltaTime);
                if (_levelView.GetStatus() != LevelStatus.Finished) return;
                State = GameStateType.GameOver;
                OpenTimeOutDialog().Forget();
            }
        }
        
        private void IntervalAutoCheckDeadlock()
        {
            if (_levelDataManager != null && _levelDataManager.IsDeadlock())
            {
                if (State == GameStateType.GameOver) return;
                State = GameStateType.GameOver;
                dragDropManager.Pause();
                OpenLoseLevelDialog().Forget();
            }
        }

        private void LoadLevel(int level)
        {
            var builder = new LevelConfigBuilder(_levelLoaderManager).SetLevel(level).Build();
            var levelView = builder.LevelObject.GetComponent<LevelView>();
            levelView.transform.SetParent(container.transform,false);

            // var levelConfig = _configManager.GetValue<PuzzleLevelConfig>(ConfigKey.LevelConfig);
            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = _levelLoaderManager.GetInputData(level);
            var levelData = levelCreator.SpawnLevel(inputData, OnItemDestroy);
            _levelDataManager = new LevelDataManager(levelData);
            _levelAnimation = new LevelAnimation(_levelDataManager, dragDropManager);
            _levelAnimation.Enter();

            levelView.Initialize(levelUI);
            _levelAnimation.SetOnShelfCleared(OnTopLayerCleared);
            _levelView = levelView;
            State = GameStateType.Loaded;
        }

        private void CleanUp()
        {
            dragDropManager.RemoveAll();
            _levelDataManager?.GetItems().ForEach(e => e?.DestroyItem());
            _levelDataManager?.Dispose();
            _levelAnimation?.Dispose();
        }

        private bool CanAcceptDropInto(IDropZone dropzone)
        {
            if (_levelDataManager == null) return false;
            var shelf = _levelDataManager.GetShelf(dropzone.ShelfId);
            if (shelf == null) return false;
            if (shelf.Type != ShelfType.Common) return false; // Chỉ cho phép drop vào Common

            var layer = _levelDataManager.GetTopLayer(dropzone.ShelfId);
            if (layer == null) return false;
            if (shelf.LockCount > 0) return false; // Check lock trước
            if (layer.Length == 0) return true; //
            if (dropzone.SlotId < 0 || dropzone.SlotId >= layer.Length) return false;
            return layer[dropzone.SlotId] == null;
        }

        private void OnItemDestroy(ShelfItemMeta itemMeta)
        {
            var itemPosition = _levelDataManager?.FindItem(itemMeta.Id).DragObject.Position;
            if (itemPosition.HasValue)
            {
                var effectPosition = new Vector3(itemPosition.Value.x, itemPosition.Value.y + 0.5f, itemPosition.Value.z); 
                EffectUtils.BlinkOnPosition(effectPosition, _levelView.gameObject);
            }
            
            dragDropManager.UnregisterDragObject(itemMeta.Id);
            _levelDataManager?.RemoveItem(itemMeta.Id);
            
            var itemLeft = _levelDataManager?.GetItems().Count;
            if (itemLeft == 0)
            {
                OnGameClear();
            }
        }
        
        private void OnTopLayerCleared(Vector2 position)
        {
            UniTask.Void(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f)); // TODO: [Refactor] - bị hard code delay sau khi hiện xong hiệu ứng Blink

                var starCount = levelUI.GetScore();

                EffectUtils.FlyMultipleStarsToUI(position, levelUI.GetStarPosition(), uiCanvas, starPrefab, starCount);

                // Tăng combo
                levelUI.IncreaseCombo();
                var combo = levelUI.GetCombo();
                var comboColor = GetComboColor(combo);
                EffectUtils.ShowComboText(position, uiCanvas, $"x{combo}", comboColor);

                await UniTask.Delay(TimeSpan.FromSeconds(0.8f));

                levelUI.AddScore();

                var storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();
                storeManager.AddStars(starCount);
            });
        }

        private ComboVFXType GetComboColor(int combo)
        {
            if (combo >= 21)
            {
                return ComboVFXType.Gradient;
            }

            var position = ((combo - 1) % 5) + 1;

            return position switch
            {
                1 => ComboVFXType.Pink,    // combo 1, 6, 11, 16
                2 => ComboVFXType.Orange,  // combo 2, 7, 12, 17
                3 => ComboVFXType.Green,   // combo 3, 8, 13, 18
                4 => ComboVFXType.Blue,    // combo 4, 9, 14, 19
                5 => ComboVFXType.Violet,  // combo 5, 10, 15, 20
                _ => ComboVFXType.Blue     // fallback
            };
        }
        
        private void OnGameClear()
        {
            if (State == GameStateType.GameOver) return;
            State = GameStateType.GameOver;
            dragDropManager.Pause();
            
            UniTask.Void(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/CompleteLevelDialog");
                var dialog = UIControllerFactory.Instance.Instantiate<CompleteLevelDialog>(prefabDialog);
                dialog.OnDidHide(BackToMenu);
                dialog.Show(canvasDialog);
            });
        }

        public void OnClickPauseButton()
        {
            if (State == GameStateType.Paused) return;
            State = GameStateType.Paused;
            dragDropManager.Pause();

            UniTask.Void(async () =>
            {
                var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/PauseGameDialog");
                var dialog = UIControllerFactory.Instance.Instantiate<PauseGameDialog>(prefabDialog);
                dialog.SetActions(() =>
                {
                    State = GameStateType.Playing;
                    dragDropManager.Unpause();
                    dialog.Hide();
                }, () =>
                {
                    dialog.Hide();
                    OpenQuitLevelDialog().Forget();
                });
                dialog.Show(canvasDialog);
            });
        }

        private async UniTaskVoid OpenQuitLevelDialog()
        {
            var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/QuitLevelDialog");
            var dialog = prefabDialog.GetComponent<ConfirmDialog>();
            dialog.SetActions(() =>
            {
                BackToMenu();
            }, () =>
            {
                dialog.Hide();
            });
            dialog.OnDidHide(() =>
            {
                State = GameStateType.Playing;
                dragDropManager.Unpause();
            });
            dialog.Show(canvasDialog);
        }
        private async UniTaskVoid OpenTimeOutDialog()
        {
            var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/TimeOutDialog");
            var dialog = prefabDialog.GetComponent<ConfirmDialog>();
            dialog.SetActions(BackToMenu, () =>
            {
                OpenSelectLevelDialog().Forget();
            });
            dialog.Show(canvasDialog);
        }

        private async UniTaskVoid OpenLoseLevelDialog()
        {
            var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/LoseLevelDialog");
            var dialog = UIControllerFactory.Instance.Instantiate<LoseLevelDialog>(prefabDialog);
            dialog.Show(canvasDialog);
        }

        private async UniTaskVoid OpenSelectLevelDialog()
        {
            var levelManager = ServiceLocator.Instance.Resolve<ILevelManager>();
            levelManager.GetCurrentLevel();
            var selectLevelDialogPrefab = await PrefabUtils.LoadPrefab("Prefabs/Dialog/SelectLevelDialog");
            var dialog = UIControllerFactory.Instance.Instantiate<SelectLevelDialog>(selectLevelDialogPrefab);
            dialog.SetCurrentLevel(levelManager.GetCurrentLevel())
                .Show(canvasDialog);
        }

        private void BackToMenu()
        {
            _audioManager.PlayMusic(AudioEnum.MenuMusic);
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<MainMenu>(nameof(MainMenu))
                .Forget();
        }
    }
}