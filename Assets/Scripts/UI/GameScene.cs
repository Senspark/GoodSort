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
using UnityEngine.UI;

namespace UI
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] private GameObject container;
        [SerializeField] private ShelfItemBasic shelfItemPrefab;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private ILevelLoaderManager _levelLoaderManager;
        private IConfigManager _configManager;

        private GameStateType State { get; set; } = GameStateType.UnInitialize;

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
            State = GameStateType.Loaded;
            dragDropManager.Init(CanAcceptDropInto);
            CleanUp();
            LoadLevel(1);
            CreateLevel(1);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _levelAnimation?.Update(dt);
        }
        
        private void LoadLevel(int level)
        {
            var builder = new LevelConfigBuilder(_levelLoaderManager).SetLevel(level).Build();
            var leveView = builder.LevelObject.GetComponent<LevelView>();
            leveView.transform.SetParent(container.transform,false);
        }

        public void CreateLevel(int level)
        {
            var levelConfig = _configManager.GetValue<PuzzleLevelConfig>(ConfigKey.LevelConfig);
            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = levelConfig.GetLevel(level).Shelves;
            // var inputData = new ShelfPuzzleInputData[]
            // {
            //     // Shelf 0: [[2], [7,9,9], [11,11,17], [19,33,33]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 2, 0, 0 },
            //             new[] { 7, 9, 9 },
            //             new[] { 11, 11, 17 },
            //             new[] { 19, 33, 33 },
            //         }
            //     },
            //
            //     // Shelf 1: [[8], [8,8,12], [14,16,16], [21,23,23]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 8, 0, 0 },
            //             new[] { 8, 8, 12 },
            //             new[] { 14, 16, 16 },
            //             new[] { 21, 23, 23 },
            //         }
            //     },
            //
            //     // Shelf 2: [[3,3], [4,10,10], [17,31], [24,25,26]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 0, 3, 3 },
            //             new[] { 4, 10, 10 },
            //             new[] { 0, 17, 31 },
            //             new[] { 24, 25, 26 },
            //         }
            //     },
            //
            //     // Shelf 3: [[2,2], [1,9,11], [18,19,19], [16,29,29]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 0, 2, 2 },
            //             new[] { 1, 9, 11 },
            //             new[] { 18, 19, 19 },
            //             new[] { 16, 29, 29 },
            //         }
            //     },
            //
            //     // Shelf 4: [[1,1,5], [7,7], [21,21,22], [25,25,26]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 1, 1, 5 },
            //             new[] { 0, 7, 7 },
            //             new[] { 21, 21, 22 },
            //             new[] { 25, 25, 26 },
            //         }
            //     },
            //
            //     // Shelf 5: [[28,28,30], [13,32], [14,17,20], [15,15,31]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 28, 28, 30 },
            //             new[] { 0, 13, 32 },
            //             new[] { 14, 17, 20 },
            //             new[] { 15, 15, 31 },
            //         }
            //     },
            //
            //     // Shelf 6: [[6,30,30], [3,4,14], [18,18,24], [15,27,24]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 6, 30, 30 },
            //             new[] { 3, 4, 14 },
            //             new[] { 18, 18, 24 },
            //             new[] { 15, 27, 24 },
            //         }
            //     },
            //
            //     // Shelf 7: [[4,6,6], [12,13,13], [20,23,26], [31,32,32]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 4, 6, 6 },
            //             new[] { 12, 13, 13 },
            //             new[] { 20, 23, 26 },
            //             new[] { 31, 32, 32 },
            //         }
            //     },
            //
            //     // Shelf 8: [[5,5,28], [10,12,29], [20,22,22], [27,27,33]]
            //     new()
            //     {
            //         Type = ShelfType.Common,
            //         Data = new[]
            //         {
            //             new[] { 5, 5, 28 },
            //             new[] { 10, 12, 29 },
            //             new[] { 20, 22, 22 },
            //             new[] { 27, 27, 33 },
            //         }
            //     },
            // };
            var levelData = levelCreator.SpawnLevel(inputData, OnItemDestroy);
            _levelDataManager = new LevelDataManager(levelData);
            _levelAnimation = new LevelAnimation(_levelDataManager, dragDropManager);
            _levelAnimation.Enter();
        }

        [Button]
        public void SetLevel(int level)
        {
            if (State <= GameStateType.Initialized)
            {
            }
            else
            {
                LoadLevel(level);
            }
        }

        private void CleanUp()
        {
            // if (!_levelView)
            // {
            //     return;
            // }
            //
            // Destroy(_levelView.gameObject);
            // _levelView = null;
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