using System;
using System.Collections.Generic;
using System.Linq;
using Engine.ShelfPuzzle;
using Newtonsoft.Json;
using Strategy.Level;

namespace Defines
{
    [Serializable]
    public class GoodsConfig
    {
        public int Id;
        public string Icon;

        // toString
        public override string ToString()
        {
            return $"GoodsConfig Id: {Id}, Icon: {Icon}";
        }
    }
    
    [Serializable]
    public class PuzzleLevelConfig
    {
        [JsonProperty("Version")] public string Version;
        [JsonProperty("Levels")] public List<PuzzleLevelData> Levels;
        public PuzzleLevelData GetLevel(int levelId)
        {
            return Levels.FirstOrDefault(e => e.Id == levelId);
        }
    }

    [Serializable]
    public class PuzzleLevelData
    {
        [JsonProperty("Id")] public int Id;
        [JsonProperty("TimeLimit")] public int TimeLimit;
        [JsonProperty("Difficulty")] public string Difficulty;
        [JsonProperty("Shelves")] public ShelfPuzzleInputData[] Shelves;
    }
}