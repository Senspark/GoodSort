using System;
using System.Collections.Generic;

namespace Defines
{
    [Serializable]
    public struct ItemData
    {
        public string item_id;
        public int quantity;
        public string display_name;
    }

    [Serializable]
    public struct PackData
    {
        public string pack_id;
        public string name;
        public int price;
        public int discount;
        public int goldAmount;
        public List<ItemData> items;
    }

    [Serializable]
    public struct ShopPacksContainer
    {
        public List<PackData> packs;
    }
}