using System;
using Newtonsoft.Json;

[Serializable]
public class LevelConfig
{
    public int Id { get; }
    public int Group { get; }
    public int Time { get; }
    public int GoodsTypeCnt { get; }
    public int LockCnt { get; }
    public int[][] ShelveMap { get; }

    [JsonConstructor]
    public LevelConfig(int id, int group, int time, int goodsTypeCnt, int lockCnt, int[][] shelveMap)
    {
        Id = id;
        Group = group;
        Time = time;
        GoodsTypeCnt = goodsTypeCnt;
        LockCnt = lockCnt;
        ShelveMap = shelveMap;
    }
}

[Serializable]
public class LevelConfigArray
{
    public LevelConfig[] Levels { get; }

    [JsonConstructor]
    public LevelConfigArray(LevelConfig[] levels)
    {
        Levels = levels;
    }
}

[Serializable]
public enum ShelveType
{
    Empty,
    Common
}

[Serializable]
public class IconConfig
{
    public int Id { get; }
    
    public string Icon { get; }
    [JsonConstructor]
    public IconConfig(int id, string icon)
    {
        Id = id;
        Icon = icon;
    }
}