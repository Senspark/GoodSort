using System;
using Newtonsoft.Json;

namespace Strategy.Level
{
    [Serializable]
    public class LevelStrategy
    {
        public int Id;
        public int TimeLimit;
        public int NormalBox;
        public int NormalLockedBox;
        public int NormalBoxLayer;
        public int RowRange;
        public int RowLockedBox;
        public int RowBoxLayer;
        public int ColumnRange;
        public int ColumnLockedBox;
        public int ColumnBoxLayer;
        public int SpecialBox;
        public int SpecialBoxLayer;
        public int Group;
        public float Density;

        [JsonConstructor]
        public LevelStrategy(int id, int timeLimit, int normalBox, int normalLockedBox, int normalBoxLayer, int rowRange,
            int rowLockedBox, int rowBoxLayer, int columnRange, int columnLockedBox, int columnBoxLayer, int specialBox,
            int specialBoxLayer, int group, float density)
        {
            Id = id;
            TimeLimit = timeLimit;
            NormalBox = normalBox;
            NormalLockedBox = normalLockedBox;
            NormalBoxLayer = normalBoxLayer;
            RowRange = rowRange;
            RowLockedBox = rowLockedBox;
            RowBoxLayer = rowBoxLayer;
            ColumnRange = columnRange;
            ColumnLockedBox = columnLockedBox;
            ColumnBoxLayer = columnBoxLayer;
            SpecialBox = specialBox;
            SpecialBoxLayer = specialBoxLayer;
            Group = group;
            Density = density;
        }
    }
}