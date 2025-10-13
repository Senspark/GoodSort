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
            // var levelObject = level.LevelObject;
            // var goodsIDArray = level.GoodsArray.Select(x => x.Id).ToList();
            //
            // _shelves = levelObject.GetComponentsInChildren<ShelveBase>().ToList();
            // var subset = DistributeGoods(goodsIDArray, level.LevelStrategy);
            // PopulateSubsetToShelve(subset);
        }

        private void PopulateSubsetToShelve(List<List<int>> subset)
        {
           
            
            var shelvesCount = _shelves.Count;

            var shelveQueue = new Dictionary<int, Queue<List<int>>>();
            for (int i = 0; i < shelvesCount; i++)
                shelveQueue[i] = new Queue<List<int>>();

            // Phân phối subsets cho các shelve
            for (int i = 0; i < subset.Count; i++)
            {
                var shelveId = i % shelvesCount;
                shelveQueue[shelveId].Enqueue(subset[i]);
            }

            // Gán queue cho từng shelve
            for (int j = 0; j < shelvesCount; j++)
            {
                var shelve = _shelves[j];
                if (!shelve) continue;

                if (shelve is CommonShelve common)
                {
                    // CommonShelve giữ nguyên
                    SetShelveQueue(common, shelveQueue[j]);
                }
                else if (shelve is SingleShelve single)
                {
                    // SingleShelve: bẻ từng số thành [x]
                    var flatQueue = new Queue<List<int>>();
                    foreach (var group in shelveQueue[j])
                    {
                        foreach (var x in group)
                            flatQueue.Enqueue(new List<int> { x });
                    }

                    SetShelveQueue(single, flatQueue);
                }
            }
        }

        private void SetShelveQueue(ShelveBase shelve, Queue<List<int>> layerQueue)
        {
            shelve.Clear();
            shelve.SetLayerQueue(layerQueue);
            shelve.LoadNextLayers();
        }

        private List<List<int>> DistributeGoods(List<int> source, LevelStrategy levelStrategy)
        {
            var totalLayer = levelStrategy.GetTotalLayer();
            var density = levelStrategy.Density;
            
            if (totalLayer < levelStrategy.Group || totalLayer > 3 * levelStrategy.Group) {
                throw new ArgumentException("Invalid input");
            }

            // mỗi số xuất hiện 3 lần
            var pool = source.ToDictionary(x => x, _ => 3);

            var subsets = new List<List<int>>();
            var rand = new System.Random();

            // 1. Nếu có single shelve thì tạo nó trước
            for (var i = 0; i < levelStrategy.SpecialBox; i++)
            {
                var first5 = source.OrderBy(_ => rand.Next()).Take(5).ToList(); 
                subsets.Add(first5);
                foreach (var x in first5) pool[x]--;
            }

            // 2. Cái nào pool[x] >= 2 ?
            var candidates = pool.Where(kv => kv.Value >= 2).Select(kv => kv.Key).ToList();
            // Tính số bộ có tính chất [x,x], [y,x,x] nè
            var specialCount = Mathf.CeilToInt(candidates.Count * density);

            // 3. Tạo special subsets
            for (int i = 0; i < specialCount; i++)
            {
                if (!candidates.Any()) break;
                var x = candidates[rand.Next(candidates.Count)];

                // chọn [x,x] hoặc [x,x,y] tùy pool
                if (pool[x] >= 2)
                {
                    if (pool[x] >= 2 && pool.Values.Sum(v => v) > 2 && rand.NextDouble() < 0.5)
                    {
                        // Tạo [x,x,y]
                        var y = pool.Where(kv => kv.Value > 0 && kv.Key != x)
                            .Select(kv => kv.Key)
                            .OrderBy(_ => rand.Next())
                            .FirstOrDefault();
                        if (y != 0)
                        {
                            subsets.Add(new List<int> { x, x, y });
                            pool[x] -= 2;
                            pool[y]--;
                        }
                        else
                        {
                            subsets.Add(new List<int> { x, x });
                            pool[x] -= 2;
                        }
                    }
                    else
                    {
                        subsets.Add(new List<int> { x, x });
                        pool[x] -= 2;
                    }
                }

                if (pool[x] < 2) candidates.Remove(x);
            }

            // 4. Tạo các subset rỗng để lấp đủ totalLayer
            while (subsets.Count < totalLayer)
                subsets.Add(new List<int>());
            // subsets.Add(new List<int>());
            // subsets.Add(new List<int>());

            // 5. Rải các phần tử còn lại trong pool vào các subset trống
            var remaining = pool.SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value)).ToList();
            foreach (var x in remaining)
            {
                var placed = subsets
                    .Where(s => s.Count < 3 && !s.Contains(x))
                    .OrderBy(_ => rand.Next())
                    .FirstOrDefault();

                if (placed != null)
                    placed.Add(x);
                else
                    throw new Exception($"Không thể đặt {x} vào tập con nào");
            }
            
            ArrayUtils.Shuffle(subsets);

            return subsets;
        }
    }
}