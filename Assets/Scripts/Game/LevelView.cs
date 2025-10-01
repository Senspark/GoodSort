using System;
using System.Collections.Generic;
using System.Linq;
using Defines;
using manager.Interface;
using Strategy.Level;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Game
{
    public interface ILevelView
    {
        public void Load(LevelConfigBuilder level);
    }

    public class LevelView : MonoBehaviour, ILevelView
    {
        private List<ShelveBase> _shelves;

        public List<ShelveBase> Shelves
        {
            get => _shelves;
            set => _shelves = value;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public void Load(LevelConfigBuilder level)
        {
            var levelObject = level.LevelObject;
            var goodsIDArray = level.GoodsArray.Select(x => x.Id).ToList();
            
            _shelves = levelObject.GetComponentsInChildren<ShelveBase>().ToList();
            var subset = DistributeGoods(goodsIDArray, level.LevelStrategy);
            PopulateSubsetToShelve(subset);
        }

        private void PopulateSubsetToShelve(List<List<int>> subset)
        {
            var shelvesCount = _shelves.Count;
            
            var shelveQueue = new Dictionary<int, Queue<List<int>>>();
            for (int i = 0; i < shelvesCount; i++)
            {
                shelveQueue[i] = new Queue<List<int>>();
            }
            
            for (int i = 0; i < subset.Count; i++)
            {
                var shelveId = i % shelvesCount;
                shelveQueue[shelveId].Enqueue(subset[i]);
            }
            
            for (var j = 0; j < shelvesCount; j++)
            {
                var shelve = _shelves[j] as CommonShelve;
                if (shelve)
                {
                    SetShelveQueue(shelve, shelveQueue[j]);
                }
            }
        }

        private void SetShelveQueue(CommonShelve shelve, Queue<List<int>> layerQueue)
        {
            shelve.Clear();
            shelve.SetLayerQueue(layerQueue);
            
            shelve.LoadNextLayers();
        }

        private List<List<int>> DistributeGoods(List<int> source, LevelStrategy levelStrategy)
        {
            var m = source.Count;
            var totalLayer = levelStrategy.MaxLayers * levelStrategy.NormalShelveCount;
            var k = levelStrategy.PairCount;

            if (k > m || totalLayer < m || totalLayer > 3 * m) {
                throw new ArgumentException("Invalid input");
            }

            var subsets = new List<List<int>>();
            var remaining = new List<int>();
            var pool = new Dictionary<int,int>();
            foreach (var x in source) pool[x] = 3;
            
            

            var pairs2 = k / 2;          // số tập [x,x]
            var pairs3 = k - pairs2;     // số tập [x,x,y]

            var idx = 0;
            // Tạo các tập con [x,x]
            for (var i = 0; i < pairs2; i++, idx++) {
                var x = source[idx];
                subsets.Add(new List<int>{ x, x });
                pool[x] -= 2;
            }
            
            // Tạo các tập con [x,x,y]
            for (var i = 0; i < pairs3; i++, idx++) {
                var x = source[idx];
                pool[x] -= 2;

                var y = -1;
                foreach (var kv in pool)
                {
                    if (kv.Value <= 0 || kv.Key == x) continue;
                    y = kv.Key;
                    break;
                }
                if (y == -1) {
                    throw new Exception("Không tìm được y để tạo [x,x,y]");
                }

                subsets.Add(new List<int>{ x, x, y });
                pool[y] -= 1;
            }
            
            foreach (var kv in pool) {
                for (var c = 0; c < kv.Value; c++) remaining.Add(kv.Key);
            }

            // Bước 2: Tạo các tập con còn lại (n - k)
            for (var i = 0; i < totalLayer - k; i++) {
                subsets.Add(new List<int>());
            }

            for (var i = 0; i < totalLayer - k; i++) {
                subsets[k + i].Add(remaining[0]);
                remaining.RemoveAt(0);
            }
            while (remaining.Count > 0) {
                var x = remaining[0];
                remaining.RemoveAt(0);

                var placed = false;
                for (var j = k; j < subsets.Count; j++)
                {
                    if (subsets[j].Count >= 3 || subsets[j].Contains(x)) continue;
                    subsets[j].Add(x);
                    placed = true;
                    break;
                }
                if (!placed) {
                    throw new Exception("Không thể đặt " + x + " vào tập con nào");
                }
            }
            return subsets;
        }
        
        
    }
}