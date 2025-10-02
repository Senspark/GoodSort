using System;

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

        public int GetTotalLayer()
        {
            return NormalBox * NormalBoxLayer + 5 * (RowRange * RowBoxLayer + ColumnRange * ColumnBoxLayer);
        }
    }

}