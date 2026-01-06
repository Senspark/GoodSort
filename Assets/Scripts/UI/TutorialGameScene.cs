using UnityEngine;
using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Defines;
using Dialog;
using Engine.ShelfPuzzle;
using Factory;
using Game;
using JetBrains.Annotations;
using manager;
using manager.Interface;
using Senspark;
using Strategy.Level;
using Tutorial;
using UnityEngine;
using Utilities;

namespace UI
{
    public class TutorialGameScene : MonoBehaviour
    {
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Canvas canvasDialog;
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] private GameObject container;
        [SerializeField] private ShelfItemBasic shelfItemPrefab;
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private Animator tutorialAnimate;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private ILevelLoaderManager _levelLoaderManager;
        private ITutorialManager _tutorialManager;
        private ITutorial _onboardingTutorial;
        private ILevelManager _levelManager;

        private GameStateType State { get; set; } = GameStateType.UnInitialize;
        private bool _didDrag = false;
        private int _levelPlaying;
        private LevelView _levelView;

        private void Awake()
        {
            try
            {
                GetServices();
            }
            catch (Exception e)
            {
                // missing service - re-initialize
                Debug.LogError(e);
            }

            return;

            void GetServices()
            {
                var services = ServiceLocator.Instance;
                _levelLoaderManager = services.Resolve<ILevelLoaderManager>();
                _tutorialManager = services.Resolve<ITutorialManager>();
                _levelManager = services.Resolve<ILevelManager>();
                State = GameStateType.Initialized;
            }
        }

        private void Start()
        {
            if (State != GameStateType.Initialized) return;
            Time.timeScale = 1;
            dragDropManager.Init(CanAcceptDropInto);
            CleanUp();
            LoadLevel(1);
            _onboardingTutorial = new OnboardingTutorial(_tutorialManager, tutorialAnimate);
            if (_onboardingTutorial.CheckStart()) _onboardingTutorial.Start();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _levelAnimation?.Update(dt);
        }

        private void LoadLevel(int level)
        {
            _levelPlaying = level;
            var builder = new LevelConfigBuilder(_levelLoaderManager).SetLevel(level).Build();
            var levelView = builder.LevelObject.GetComponent<LevelView>();
            _levelView = levelView;
            _levelView.transform.SetParent(container.transform,false);

            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = _levelLoaderManager.GetInputData(level);
            var levelData = levelCreator.SpawnLevel(inputData, OnItemDestroy);
            _levelDataManager = new LevelDataManager(levelData);
            _levelAnimation = new LevelAnimation(_levelDataManager, dragDropManager);
            _levelAnimation.Enter();

            _levelAnimation.SetOnShelfCleared((_) =>
            {
                TutorialEvents.SendEvent(TutorialActionType.OnClear);
            });
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
        
        private void OnGameClear()
        {
            if (State == GameStateType.GameOver) return;
            State = GameStateType.GameOver;
            dragDropManager.Pause();
            
            // Track: Win Level
            // _analyticsManager?.PopGameLevel(true);
            UniTask.Void(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                if (_levelPlaying == 1)
                {
                    _levelManager.GoToNextLevel();
                    GoToGameScene();
                }
                // var prefabDialog = await PrefabUtils.LoadPrefab("Prefabs/Dialog/CompleteLevelDialog");
                // var dialog = UIControllerFactory.Instance.Instantiate<CompleteLevelDialog>(prefabDialog);
                // dialog.OnDidHide(BackToMenu);
                // dialog.Show(canvasDialog);
            });
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
        
        private void GoToGameScene()
        {
            ServiceLocator.Instance
                .Resolve<ISceneLoader>()
                .LoadScene<GameScene>("GameScene")  // Tên scene trong Build Settings
                .Forget();
        }
    }
}