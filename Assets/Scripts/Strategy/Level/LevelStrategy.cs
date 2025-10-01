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
        public int RowBox;
        public int RowLockedBox;
        public int RowBoxLayer;
        public int ColumnBox;
        public int ColumnLockedBox;
        public int ColumnBoxLayer;
        public int SpecialBox;
        public int SpecialBoxLayer;
        public int Group;
        public float Density;

        [JsonConstructor]
        public LevelStrategy(int id, int timeLimit, int normalBox, int normalLockedBox, int normalBoxLayer, int rowBox,
            int rowLockedBox, int rowBoxLayer, int columnBox, int columnLockedBox, int columnBoxLayer, int specialBox,
            int specialBoxLayer, int group, float density)
        {
            Id = id;
            TimeLimit = timeLimit;
            NormalBox = normalBox;
            NormalLockedBox = normalLockedBox;
            NormalBoxLayer = normalBoxLayer;
            RowBox = rowBox;
            RowLockedBox = rowLockedBox;
            RowBoxLayer = rowBoxLayer;
            ColumnBox = columnBox;
            ColumnLockedBox = columnLockedBox;
            ColumnBoxLayer = columnBoxLayer;
            SpecialBox = specialBox;
            SpecialBoxLayer = specialBoxLayer;
            Group = group;
            Density = density;
        }
    }
}