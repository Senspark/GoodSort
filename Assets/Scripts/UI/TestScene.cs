using Core;
using Engine.ShelfPuzzle;
using Game;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Strategy.Level;
using UnityEngine;

namespace UI
{
    public class TestScene : MonoBehaviour
    {
        [SerializeField] private DragDropGameManager dragDropManager;
        [SerializeField] public GameObject container;
        [SerializeField] public ShelfItemBasic shelfItemPrefab;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

        private void Start()
        {
            CreateLevel();
        }

        [Button]
        public void CreateLevel()
        {
            CleanUp();
            var levelCreator = new LevelCreator(container, shelfItemPrefab);
            var inputData = new ShelfPuzzleInputData[]
            {
                new()
                {
                    Type = ShelfType.Common,
                    Data = new[]
                    {
                        new[] { 1, 1, 0 },
                        new[] { 2, 2, 0 }
                    }
                },
                new()
                {
                    Type = ShelfType.Common,
                    Data = new[]
                    {
                        new[] { 0, 2, 1 },
                    }
                }
            };
            var levelData = levelCreator.SpawnLevel(inputData, OnItemDestroy);
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

        private void OnItemDestroy(ShelfItemMeta itemMeta)
        {
            dragDropManager.UnregisterDragObject(itemMeta.Id);
            _levelDataManager?.RemoveItem(itemMeta.Id);
        }
    }
}