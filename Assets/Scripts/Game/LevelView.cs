using System;
using System.Collections.Generic;
using System.Linq;
using Engine.ShelfPuzzle;
using Strategy.Level;
using UnityEngine;
using Utilities;

namespace Game
{
    public interface ILevelView
    {
        public void Load(LevelConfigBuilder level);
        public ShelfPuzzleInputData[] ExportData();
    }

    public class LevelView : MonoBehaviour, ILevelView
    {
        public List<ShelveBase> Shelves { get; private set; }
        private ShelfPuzzleInputData[] _input;
        private const int MaxItem = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public void Load(LevelConfigBuilder level)
        {
            var levelObject = level.LevelObject;
            var goodsIDArray = level.GoodsArray.Select(x => x.Id).ToList();

            Shelves = levelObject.GetComponentsInChildren<ShelveBase>().ToList();
            Sort(Shelves);
            var subset = DistributeGoods(goodsIDArray, level.LevelStrategy);
            _input = PopulateSubsetToShelve(Shelves, subset);
        }

        public ShelfPuzzleInputData[] ExportData()
        {
            return _input;
        }

        private static ShelfPuzzleInputData[] PopulateSubsetToShelve(List<ShelveBase> shelves, List<List<int>> subset)
        {
            var shelvesCount = shelves.Count;

            for (var i = 0; i < shelvesCount; i++)
            {
                //Log subset 
                Debug.Log($"Subset {i}: [{string.Join(",", subset[i])}]");
            }

            var shelveQueue = new Dictionary<int, List<List<int>>>();
            for (var i = 0; i < shelvesCount; i++)
            {
                shelveQueue[i] = new List<List<int>>();
            }

            // Phân phối subsets cho các shelve
            for (var i = 0; i < subset.Count; i++)
            {
                var shelveId = i % shelvesCount;
                shelveQueue[shelveId].Add(subset[i]);
            }

            // Gán queue cho từng shelve
            var exportedData = new ShelfPuzzleInputData[shelvesCount];
            for (var id = 0; id < shelvesCount; id++)
            {
                var shelve = shelves[id];
                if (!shelve) continue;

                switch (shelve)
                {
                    case CommonShelve common:
                        AddItemForCommonShelf(shelveQueue, id, exportedData, common);
                        break;
                    case SingleShelve single:
                        AddItemForSingleShelf(shelveQueue, id, exportedData, single);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Shelf chưa quy định: {shelve}");
                }
            }

            return exportedData;
        }

        private static void AddItemForSingleShelf(
            Dictionary<int, List<List<int>>> shelveQueue, int id,
            ShelfPuzzleInputData[] exportedData,
            SingleShelve single
        )
        {
            // SingleShelve: bẻ từng số thành [x]
            var flat = new List<int[]>();
            foreach (var layer in shelveQueue[id])
            {
                foreach (var item in layer)
                {
                    flat.Add(new[] { item });
                }
            }

            var input = exportedData[id] = new ShelfPuzzleInputData
            {
                Type = ShelfType.TakeOnly,
                Data = flat.ToArray()
            };

            SetShelveQueue(id, single, input);
        }

        private static void AddItemForCommonShelf(
            Dictionary<int, List<List<int>>> shelveQueue, int id,
            ShelfPuzzleInputData[] exportedData,
            CommonShelve common
        )
        {
            var list = new List<int[]>();
            foreach (var layer in shelveQueue[id])
            {
                // Nếu chưa đủ 3 item thì thêm vào
                var newLayer = new int[MaxItem];
                for (var i = 0; i < MaxItem; i++)
                {
                    if (i + 1 <= layer.Count)
                    {
                        newLayer[i] = layer[i];
                    }
                    else
                    {
                        newLayer[i] = 0;
                    }
                }

                list.Add(newLayer);
            }

            var array = list.ToArray();
            ArrayUtils.Shuffle(array);

            var input = exportedData[id] = new ShelfPuzzleInputData
            {
                Type = ShelfType.Common,
                Data = array
            };
            SetShelveQueue(id, common, input);
        }

        private static void SetShelveQueue(int shelfId, ShelveBase shelve, ShelfPuzzleInputData input)
        {
            shelve.Clear();
            shelve.SetLayerQueue(shelfId, input);
            shelve.LoadNextLayers();
        }

        private static List<List<int>> DistributeGoods(List<int> source, LevelStrategy levelStrategy)
        {
            var totalLayer = levelStrategy.GetTotalLayer();
            var density = levelStrategy.Density;

            Debug.Log($"Total layer: {totalLayer}, Density: {density}");

            if (totalLayer < levelStrategy.Group || totalLayer > 3 * levelStrategy.Group)
            {
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
            {
                subsets.Add(new List<int>());
            }
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

            // Log ra thử coi có đúng ko  
            foreach (var (subset, idx) in subsets.Select((s, i) => (s, i)))
            {
                Debug.Log($"Subset {idx}: [{string.Join(",", subset)}]");
            }

            // Shuffle subsets
            ArrayUtils.Shuffle(subsets);

            return subsets;
        }

        /**
         * Sort by: Y position from High to Low, X Position From Left To Right
         */
        private static void Sort(List<ShelveBase> shelves)
        {
            shelves.Sort((a, b) =>
            {
                var yCompare = b.transform.position.y.CompareTo(a.transform.position.y); // High to Low
                if (yCompare != 0) return yCompare;
                return a.transform.position.x.CompareTo(b.transform.position.x); // Left to Right
            });

            // Re-order sibling transforms to match sorted order
            for (var i = 0; i < shelves.Count; i++)
            {
                shelves[i].transform.SetSiblingIndex(i);
            }
        }
    }
}