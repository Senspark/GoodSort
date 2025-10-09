using Core;
using Cysharp.Threading.Tasks;
using Engine.ShelfPuzzle;
using Game;
using JetBrains.Annotations;
using manager;
using Sirenix.OdinInspector;
using Strategy.Level;
using UnityEngine;

namespace UI
{
    public class TestScene : MonoBehaviour
    {
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] public GameObject container;
        [SerializeField] public ShelfItemBasic shelfItemPrefab;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private ShelfPuzzleInputData[] _inputData;

        private void Start()
        {
            dragDropManager.Init(CanAcceptDropInto);
            CreateLevel();
        }

        [Button]
        public void CreateLevel()
        {
            CleanUp();
            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            _inputData = new ShelfPuzzleInputData[]
            {
                new()
                {
                    Type = ShelfType.Common,
                    Data = new[]
                    {
                        new[] { 1, 1, 0 },
                        new[] { 2, 2, 0 },
                        new[] { 0, 3, 0 },
                    }
                },
                new()
                {
                    Type = ShelfType.Common,
                    Data = new[]
                    {
                        new[] { 0, 2, 1 },
                    }
                },
                new()
                {
                    Type = ShelfType.TakeOnly,
                    Data = new[]
                    {
                        new[] { 3, 3 }
                    }
                }
            };
            var levelData = levelCreator.SpawnLevel(_inputData, OnItemDestroy);
            _levelDataManager = new LevelDataManager(levelData);
            _levelAnimation = new LevelAnimation(_levelDataManager, dragDropManager);
            _levelAnimation.Enter();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _levelAnimation?.Update(dt);
        }

        private void CleanUp()
        {
            dragDropManager.RemoveAll();
            _levelDataManager?.GetItems().ForEach(e => e?.DestroyItem());
            _levelDataManager?.Dispose();
            _levelAnimation?.Dispose();
        }

        private bool CanAcceptDropInto(IDropZone dropZone)
        {
            if (_levelDataManager == null) return false;
            var layer = _levelDataManager.GetTopLayer(dropZone.ShelfId);
            if (dropZone.SlotId < 0 || dropZone.SlotId >= layer.Length) return false;
            return layer[dropZone.SlotId] == null;
        }

        private void OnItemDestroy(ShelfItemMeta itemMeta)
        {
            dragDropManager.UnregisterDragObject(itemMeta.Id);
            _levelDataManager?.RemoveItem(itemMeta.Id);
        }

        [Button]
        private void AutoSolve()
        {
            var logger = new AppendLogger();

            UniTask.Void(async () =>
            {
                await UniTask.SwitchToThreadPool();
                var solution = new PuzzleSolver(logger).SolvePuzzleWithStateChanges(_inputData);

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