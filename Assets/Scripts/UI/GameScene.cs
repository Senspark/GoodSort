using System;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Defines;
using Engine.ShelfPuzzle;
using Game;
using JetBrains.Annotations;
using manager;
using manager.Interface;
using Senspark;
using Sirenix.OdinInspector;
using Strategy.Level;
using UnityEngine;

namespace UI
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] private GameObject container;
        [SerializeField] private ShelfItemBasic shelfItemPrefab;
        [SerializeField] private LevelUI levelUI;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private ILevelLoaderManager _levelLoaderManager; 
        private IConfigManager _configManager;

        private GameStateType State { get; set; } = GameStateType.UnInitialize;
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
                MockGameInitializer.Initialize()
                    .ContinueWith(() =>
                    {
                        GetServices();
                        Start();
                    })
                    .Forget();
            }

            return;

            void GetServices()
            {
                var services = ServiceLocator.Instance;
                _levelLoaderManager = services.Resolve<ILevelLoaderManager>();
                _configManager = services.Resolve<IConfigManager>();
                State = GameStateType.Initialized;
            }
        }

        private void Start()
        {
            if (State != GameStateType.Initialized) return;
            Time.timeScale = 1;
            State = GameStateType.Loaded;
            dragDropManager.Init(CanAcceptDropInto);
            dragDropManager.SetOnDragStarted(OnDragStarted);
            CleanUp();
            LoadLevel(3);
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
            if (State == GameStateType.Started)
            {
                if (_levelView)                                                             
                    _levelView.Step(Time.deltaTime);
            }
            
        }
        
        private void LoadLevel(int level)
        {
            var builder = new LevelConfigBuilder(_levelLoaderManager).SetLevel(level).Build();
            var leveView = builder.LevelObject.GetComponent<LevelView>();
            leveView.transform.SetParent(container.transform,false);
            
            var levelConfig = _configManager.GetValue<PuzzleLevelConfig>(ConfigKey.LevelConfig);
            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = levelConfig.GetLevel(level).Shelves;
            var levelData = levelCreator.SpawnLevel(inputData, OnItemDestroy);
            _levelDataManager = new LevelDataManager(levelData);
            _levelAnimation = new LevelAnimation(_levelDataManager, dragDropManager);
            _levelAnimation.Enter();
            
            leveView.Initialize(levelUI, levelConfig.GetLevel(level));
            _levelView = leveView;
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
            if (layer.Length == 0) return true; // Nếu layer rỗng thì có thể drop vào

            if (dropzone.SlotId < 0 || dropzone.SlotId >= layer.Length) return false;
            return layer[dropzone.SlotId] == null;
        }

        private void OnItemDestroy(ShelfItemMeta itemMeta)
        {
            dragDropManager.UnregisterDragObject(itemMeta.Id);
            _levelDataManager?.RemoveItem(itemMeta.Id);
        }
        
        private void OnDragStarted(IDragObject dragObject)
        {
            if (State > GameStateType.Started) return;
            State = GameStateType.Started;
            // Chuyển state Started khi bắt đầu drag lần đầu tiên
        }
        
        [Button]
        private void AutoSolve()
        {
            if (_levelDataManager == null) return;
            var logger = new AppendLogger();

            UniTask.Void(async () =>
            {
                await UniTask.SwitchToThreadPool();
                var exportedData = _levelDataManager.Export();
                var solution = new PuzzleSolver(logger).SolvePuzzleWithStateChanges(exportedData);

                await UniTask.SwitchToMainThread();
                logger.PrintLogs();

                var autoplay = gameObject.GetComponent<AutoPlay2>();
                if (!autoplay)
                {
                    autoplay = gameObject.AddComponent<AutoPlay2>();
                }

                autoplay.Play(_levelDataManager, dragDropManager, solution);
            });
        }
    }
}