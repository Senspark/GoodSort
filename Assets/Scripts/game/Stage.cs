using UnityEngine;
using manager;
using Utilities;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

namespace game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] private GameObject shelveContainer;
        [SerializeField] private GameObject[] shelvePrefabArray;

        private LevelConfig _levelConfig;
        private const int FixedWidth = 348;
        private const int FixedHeight = 300;
        private ShelveBase[] _listShelve;

        private void Start()
        {
            _levelConfig = GameManager.Instance.GetCurrentLevelConfig();
            CreateShelves(_levelConfig.ShelveMap);
            // CreateShelveLockers(_levelConfig.lockCnt);
            CreateGoodsArray(_levelConfig.Group, _levelConfig.GoodsTypeCnt);
            CreateGoodsData();

            StartCoroutine(OnCheckWin());
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
            _listShelve = shelveContainer.GetComponentsInChildren<ShelveBase>();
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
            var placeCount = _listShelve.Length * 3;
            while (GameManager.Instance.GoodsArray.Count > 0)
            {
                List<int> placeList = new List<int>();
                for (var i = 0; i < placeCount; i++)
                {
                    placeList.Add(i);
                }
                ArrayUtility.Shuffle(placeList);

                placeList.RemoveRange(0, placeCount <= 6 ? Random.Range(2, 3) : Random.Range(4, 6));
                for (int i = 0; i < placeCount; i++)
                {
                    var placeId = i;
                    var shelveId = _listShelve[Mathf.FloorToInt(placeId / 3)].Id;
                    var slotId = placeId % 3;
                    if(!GameManager.Instance.GoodsData.ContainsKey(shelveId))
                    {
                        GameManager.Instance.GoodsData[shelveId] = new Dictionary<int, List<int>>()
                        {
                            { 0, new List<int>() },
                            { 1, new List<int>() },
                            { 2, new List<int>() }
                        };
                    }

                    var goodsId = -1;
                    if (GameManager.Instance.GoodsArray.Count <= 0 || !placeList.Contains(placeId))
                    {
                        goodsId = -1;
                    }
                    else
                    {
                        goodsId = GameManager.Instance.GoodsArray[0];
                        GameManager.Instance.GoodsArray.RemoveAt(0);
                    }
                    GameManager.Instance.GoodsData[shelveId][slotId].Add(goodsId);
                }
            }
        }

        private IEnumerator OnCheckWin()
        {
            while (true)
            {
                if (CheckWin())
                {
                    Debug.Log("Win");
                    yield break; // dừng coroutine
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        private bool CheckWin()
        {
            return _listShelve.All(shelve => shelve.IsEmpty());
        }
    }
}