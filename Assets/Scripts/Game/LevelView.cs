using System;
using System.Collections.Generic;
using System.Linq;
using Defines;
using manager.Interface;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Game
{
    public interface ILevelView
    {
        public void Load(LevelConfigBuilder level, IEventManager eventManager);
    }
    public class LevelView : MonoBehaviour, ILevelView
    {
        private List<ShelveBase> _shelves;
        private List<GoodsConfig> _goodsArray;
        
        public void Load(LevelConfigBuilder level, IEventManager eventManager)
        {
            var levelObject = level.LevelObject;
            var tripleCnt = level.LevelStrategy.TripleCount;
            var pairCnt = level.LevelStrategy.PairCount;
            _shelves = levelObject.GetComponentsInChildren<ShelveBase>().ToList();
            _goodsArray = CreateGoodsArray(level.GoodsArray, tripleCnt, pairCnt);
        }
        
        private List<GoodsConfig> CreateGoodsArray(List<GoodsConfig> goodsArray, int tripleCount, int pairCount)
        {
            var store = ArrayUtils.GenerateRandomList(goodsArray, tripleCount);
            Dictionary<int, int> goodsCount = new();
            foreach (var goods in store)
            {
                goodsCount[goods.Id] = 3;
            }
            
            List<List<GoodsConfig>> boxes = new();
            for (int i = 0; i < pairCount; i++)
            {
                var pair = store[i];
                var one = store[(i + 1) % tripleCount];
                boxes.Add(new List<GoodsConfig> { pair, pair, one});
                goodsCount[pair.Id] -= 2;
                goodsCount[one.Id] -= 1;
            }
            
            List<GoodsConfig> remain = new();
            foreach (var kvp in goodsCount)
            {
                var id = kvp.Key;
                var count = kvp.Value;
                var template = store.First(g => g.Id == id);
                for (var i = 0; i < count; i++)
                {
                    remain.Add(template);
                }
            }

            var remainGroups = goodsCount
                .SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
                .GroupBy(id => id)
                .ToDictionary(g => g.Key, g => g.Count());
    
            // Tạo boxes còn lại với 3 ID khác nhau
            for (var i = 0; i < tripleCount - pairCount; i++)
            {
                var box = new List<int>();
        
                var availableIds = remainGroups.Where(kv => kv.Value > 0)
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .ToList();
        
                for (int j = 0; j < Math.Min(3, availableIds.Count); j++)
                {
                    box.Add(availableIds[j]);
                    remainGroups[availableIds[j]]--;
                }
        
                // Thêm box vào kết quả
                boxes.Add(box.Select(id => store.First(g => g.Id == id)).ToList());
            }
            
            // Fisher-Yates shuffle
            for (int i = boxes.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (boxes[i], boxes[j]) = (boxes[j], boxes[i]);
            }
            // return flat
            return boxes.SelectMany(x => x).ToList();
        }
    }
}