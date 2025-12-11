using System.Collections.Generic;
using Defines;
using TMPro;
using UnityEngine;

namespace UI.Menu.Shop
{
    public class ShopPack
    {
        [SerializeField] private TMP_Text packageName;
        [SerializeField] private int price;
        [SerializeField] private int discount;
        [SerializeField] private int goldAmount;
        [SerializeField] private List<ItemData> items;
    }
}