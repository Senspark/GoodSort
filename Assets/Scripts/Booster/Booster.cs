using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Booster
{
    public enum BoosterType
    {
        MagicHat = 0,
        MagicStaff = 1,
        Freeze = 2,
        Magnet = 3,
        Live = 4,
        MoreTime = 5,
        Swap = 6,
        X2Star = 7
    }

    public class Booster : MonoBehaviour
    {
        [SerializeField] private Image boosterIcon;
        [SerializeField] private TMP_Text quantityText;

        private BoosterType _type;
        private int _quantity;

        private static readonly Dictionary<BoosterType, string> BoosterIconNames = new Dictionary<BoosterType, string>
        {
            { BoosterType.MagicHat, "icon_booster_hat" },
            { BoosterType.MagicStaff, "icon_booster_staff" },
            { BoosterType.Freeze, "icon_booster_freeze" },
            { BoosterType.Magnet, "icon_booster_Magnet" },
            { BoosterType.Live, "icon_booster_hat" }, // Using hat as placeholder, update when Live icon is available
            { BoosterType.MoreTime, "icon_booster_clock" },
            { BoosterType.Swap, "icon_booster_reverse" },
            { BoosterType.X2Star, "icon_booster_x2" }
        };

        /// <summary>
        /// Set booster type and update icon
        /// </summary>
        public Booster SetType(BoosterType type)
        {
            _type = type;
            UpdateIcon();
            return this;
        }

        /// <summary>
        /// Set booster quantity and update display
        /// </summary>
        public Booster SetQuantity(int quantity)
        {
            _quantity = quantity;
            UpdateQuantityText();
            return this;
        }

        /// <summary>
        /// Get current booster type
        /// </summary>
        public BoosterType GetBoosterType() => _type;

        private void UpdateIcon()
        {
            if (boosterIcon == null)
            {
                Debug.LogWarning("[Booster] boosterIcon is null!");
                return;
            }

            if (!BoosterIconNames.TryGetValue(_type, out string iconName))
            {
                Debug.LogError($"[Booster] No icon name mapping found for {_type}");
                return;
            }

            var iconPath = $"BoosterIcon/{iconName}";
            var loadedSprite = Resources.Load<Sprite>(iconPath);

            if (loadedSprite == null)
            {
                Debug.LogError($"[Booster] Failed to load icon from path: {iconPath}");
                return;
            }

            boosterIcon.sprite = loadedSprite;
        }

        private void UpdateQuantityText()
        {
            quantityText.text = "x" + _quantity;
        }
    }
}