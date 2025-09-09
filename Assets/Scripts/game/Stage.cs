using UnityEngine;
using manager;
using Utilities;
using System.Collections.Generic;

namespace game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] private GameObject shelveContainer;
        [SerializeField] private GameObject[] shelvePrefabArray;

        private LevelConfig _levelConfig;
        private const int FixedWidth = 348;
        private const int FixedHeight = 300;

        private void Start()
        {
            _levelConfig = GameManager.Instance.GetCurrentLevelConfig();
            CreateShelves(_levelConfig.ShelveMap);
            // CreateShelveLockers(_levelConfig.lockCnt);
            CreateGoodsArray(_levelConfig.Group, _levelConfig.GoodsTypeCnt);
            CreateGoodsData();

            StartCoroutine(CheckWin());
        }

        private void CreateShelves(int[][] shelveMap)
        {
            var row = shelveMap.Length;
            var col = shelveMap[0].Length;
            var uiTransform = shelveContainer.GetComponent<RectTransform>();
            // set width and height of shelveContainer to row * 100 and col * 100
            uiTransform.sizeDelta = new Vector2(col * FixedWidth, row * FixedHeight);
            for (var i = row - 1; i >= 0; i--)
            {
                for (var j = 0; j < col; j++)
                {
                    var type = (ShelveType)shelveMap[i][j];
                    var shelvePrefab = shelvePrefabArray[(int)type];
                    Instantiate(shelvePrefab, shelveContainer.transform);
                }
            }
        }
        
        private void CreateGoodsArray(int group, int goodsTypeCnt)
        {
            var goodsArray = GameManager.Instance.IconConfigs;
            var indexArray = ArrayUtility.GenerateRandomArrayIndexNoDuplicate(goodsArray, goodsTypeCnt);
            // clear good array in GameManager
            GameManager.Instance.GoodsArray = new List<int>();
            for (var i = 0; i < group; i++)
            {
                var goodsId = ArrayUtility.GetRandomElement(indexArray);
                GameManager.Instance.GoodsArray.Add(goodsId + 1);
                GameManager.Instance.GoodsArray.Add(goodsId + 1);
                GameManager.Instance.GoodsArray.Add(goodsId + 1);
            }
            ArrayUtility.Shuffle(GameManager.Instance.GoodsArray);
        }
        
        private void CreateGoodsData()
        {
            // get all shelve in shelveContainer
            var shelveList = shelveContainer.GetComponentsInChildren<ShelveBase>();
            // get all goods in GameManager
            var goodsArray = GameManager.Instance.GoodsArray;
            // loop through shelveList
            foreach (var shelve in shelveList)
            {
                // loop through goodsArray
    }
}