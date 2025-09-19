using System;
using Newtonsoft.Json;

namespace Strategy.Level
{
    [Serializable]
    public class LevelStrategy
    {
        public int Id { get; set; }
        public int TimeLimit { get; set; }
        public int NormalShelveCount { get; set; }
        public int LockedShelveCount { get; set; }
        public int MaxLayers { get; set; }
        public int MinLayers { get; set; }
        public int SpecialShelveCount { get; set; }
        public int GoodsPerSpecialShelve { get; set; }
        public int TripleCount { get; set; }
        public int PairCount { get; set; }

        [JsonConstructor]
        public LevelStrategy(int id, 
            int time_limit, 
            int normal_count, 
            int locked_count, 
            int max_layers,
            int min_layers, 
            int special_count, 
            int goods_per_special_shelve, 
            int triple_count, 
            int pair_count)
        {
            Id = id;
            TimeLimit = time_limit;
            NormalShelveCount = normal_count;
            LockedShelveCount = locked_count;
            MaxLayers = max_layers;
            MinLayers = min_layers;
            SpecialShelveCount = special_count;
            GoodsPerSpecialShelve = goods_per_special_shelve;
            TripleCount = triple_count;
            PairCount = pair_count;
        }
    }
}