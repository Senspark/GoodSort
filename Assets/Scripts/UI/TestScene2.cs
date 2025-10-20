using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Engine.ShelfPuzzle;
using Game;
using JetBrains.Annotations;
using manager;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Strategy.Level;
using UnityEngine;

namespace UI
{
    public class TestScene2 : MonoBehaviour
    {
        [SerializeField] private DragDropManager2 dragDropManager;
        [SerializeField] public GameObject container;
        [SerializeField] public ShelfItemBasic shelfItemPrefab;
        [SerializeField] public TextAsset levelJsonFile;
        [SerializeField] public TextAsset levelSolutionJsonFile;

        [CanBeNull] private LevelDataManager _levelDataManager;
        [CanBeNull] private LevelAnimation _levelAnimation;

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
            var inputData = JsonConvert.DeserializeObject<ShelfPuzzleInputData[]>(levelJsonFile.text);
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

        private bool CanAcceptDropInto(IDropZone dropZone)
        {
            if (_levelDataManager == null) return false;
            var shelf = _levelDataManager.GetShelf(dropZone.ShelfId);
            if (shelf == null) return false;
            if (shelf.Type != ShelfType.Common) return false; // Chỉ cho phép drop vào Common

            var layer = _levelDataManager.GetTopLayer(dropZone.ShelfId);
            if (layer == null) return false;
            if (layer.Length == 0) return true; // Nếu layer rỗng thì có thể drop vào

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
            if (_levelDataManager == null) return;
            var moves = JsonConvert.DeserializeObject<Move[]>(levelSolutionJsonFile.text);

            var autoplay = gameObject.GetComponent<AutoPlay2>();
            if (!autoplay)
            {
                autoplay = gameObject.AddComponent<AutoPlay2>();
            }

            autoplay.Play(_levelDataManager, dragDropManager, moves);
        }
    }
}