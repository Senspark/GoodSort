using System;
using System.Collections.Generic;
using Core;
using Engine.ShelfPuzzle;
using manager;
using Newtonsoft.Json;

namespace Strategy.Level
{
    public static class LevelImporter
    {
        public static void Import(List<IShelf> shelves, string json)
        {
            try
            {
                var level = JsonConvert.DeserializeObject<ShelfPuzzleInputData[]>(json);
                if (shelves.Count != level.Length)
                {
                    CleanLogger.Error("Level không phù hợp với Data");
                    return;
                }

                SpawnCommonLevel(shelves, level);
            }
            catch (Exception e)
            {
                CleanLogger.Error(e);
            }
        }

        private static void SpawnCommonLevel(List<IShelf> shelves, ShelfPuzzleInputData[] levelData)
        {
            for (var id = 0; id < levelData.Length; id++)
            {
                var shelf = shelves[id];
                var shelfData = levelData[id];
                
                shelf.ImportData(id, shelfData);
            }
        }
    }
}