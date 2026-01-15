using System;
using System.Collections.Generic;
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
    public interface GameController
    {
        void AddStar(Vector3 position);
    }
    public class GameScene : MonoBehaviour, GameController
    {
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private LevelView levelView;
        [SerializeField] private CollectionStar collectionStar;
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] private GameObject container;
        [SerializeField] private ShelfItemBasic shelfItemPrefab;
        [SerializeField] private GameObject starPrefab;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private ILevelLoaderManager _levelLoaderManager;
        private ILevelManager _levelManager;
        private IAudioManager _audioManager;
        private IAnalyticsManager _analyticsManager;
        private IStoreManager _storeManager;

        private GameStateType State { get; set; } = GameStateType.UnInitialize;
        private bool _didDrag;
        private int _levelPlaying;
        private ComboData _comboData;

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
            _audioManager.PlayMusic(AudioEnum.GameMusic);
            return;

            void GetServices()
            {
                var services = ServiceLocator.Instance;
                _levelLoaderManager = services.Resolve<ILevelLoaderManager>();
                _levelManager = services.Resolve<ILevelManager>();
                _audioManager = services.Resolve<IAudioManager>();
                _analyticsManager = services.Resolve<IAnalyticsManager>();
                _storeManager = services.Resolve<IStoreManager>();
                State = GameStateType.Initialized;
            }
        }

        private void Start()
        {
            _ = SceneTransition.Instance.FadeOut();
            if (State != GameStateType.Initialized) return;
            Time.timeScale = 1;
            dragDropManager.Init(CanAcceptDropInto);
            CleanUp();
            LoadLevel(_levelManager.GetCurrentLevel());
            InvokeRepeating(nameof(IntervalAutoCheckDeadlock), 5f, 3f);
            _analyticsManager?.PushGameLevel(_levelPlaying, "normal");
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _levelAnimation?.Update(dt);
            if (State == GameStateType.Paused) return;
            if (!_didDrag && dragDropManager.IsDragging())
            {
                if (State < GameStateType.Playing)
                {
                    _didDrag = true;
                    State = GameStateType.Playing;
                    OnStart();
                }
            }
            if (State == GameStateType.Playing)
            {
                _comboData.Step(Time.deltaTime);
            }
        }

        private void OnStart()
        {
            levelView.SimulationManager.Paused = false;
            levelView.Status = LevelStatus.Playing;
        }
        private void IntervalAutoCheckDeadlock()
        {
            if (_levelDataManager != null && _levelDataManager.IsDeadlock())
            {
                if (State == GameStateType.GameOver) return;
                State = GameStateType.GameOver;
                dragDropManager.Pause();
                _analyticsManager?.PopGameLevel(false);
                OpenLoseLevelDialog().Forget();
            }
        }

        private void LoadLevel(int level)
        {
            _levelPlaying = level;
            var builder = new LevelConfigBuilder(_levelLoaderManager).SetLevel(level).Build();
            var levelTransform = builder.LevelObject.GetComponent<Transform>();
            levelTransform.SetParent(container.transform,false);

            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = _levelLoaderManager.GetInputData(level);
            var levelData = levelCreator.SpawnLevel(inputData, OnItemDestroy);
            _levelDataManager = new LevelDataManager(levelData);
            _levelAnimation = new LevelAnimation(_levelDataManager, dragDropManager)
            {
                GameSceneController = this
            };
            _levelAnimation.Enter();
            State = GameStateType.Loaded;

            var duration = 360;
            var boosters = new Booster.Booster[] { };
            levelView.SetConfig(duration, boosters);
            
            _comboData = new ComboData(_storeManager);
            levelView.SetComboData(_comboData);
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
            dragDropManager.UnregisterDragObject(itemMeta.Id);
            _levelDataManager?.RemoveItem(itemMeta.Id);
            
            var itemLeft = _levelDataManager?.GetItems().Count;
            if (itemLeft == 0)
            {
                OnGameClear();
            }
        }
        
        public void AddStar(Vector3 position)
        {
            _comboData.ComboUp();
            var uiLocalPos = WorldToUISpace(uiCanvas, position);
            EffectUtils.ShowComboText(uiLocalPos, uiCanvas, _comboData.Combo);
            _comboData.AddScore();
            collectionStar.CollectionStars(uiLocalPos, _comboData.CalculateScore());
        }
        
        private void OnGameClear()
        {
            if (State == GameStateType.GameOver) return;
            State = GameStateType.GameOver;
            dragDropManager.Pause();
            
            // Track: Win Level
            _analyticsManager?.PopGameLevel(true);
            UniTask.Void(async () =>
            {
                Debug.Log("UniTask.Void(async");
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/CompleteLevelDialog");
                var dialog = UIControllerFactory.Instance.Instantiate<CompleteLevelDialog>(prefabDialog);
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
                // Track: Quit Level
                _analyticsManager?.PopGameLevel(false);
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
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<MainMenu>(nameof(MainMenu))
                .Forget();
        }
        
        private void OnDestroy()
        {
            CancelInvoke(nameof(IntervalAutoCheckDeadlock));
        }
        
        // Helper function
        private Vector2 WorldToUISpace(Canvas canvas, Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                null,
                out var localPos
            );
            return localPos;
        }
    }
}