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
        [Header("Property")]
        [SerializeField] private Image boosterIcon;
        [SerializeField] private TMP_Text quantityText;

        [Header("Icons")]
        [Tooltip("Order: MagicHat, MagicStaff, Freeze, Magnet, Live, MoreTime, Swap, X2Star")]
        [SerializeField] private Sprite[] boosterIcons;

        private BoosterType _type;
        private int _quantity;

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

            int iconIndex = (int)_type;
            if (boosterIcons == null || iconIndex >= boosterIcons.Length)
            {
                Debug.LogError($"[Booster] Icon for {_type} not found! Index: {iconIndex}, Array length: {boosterIcons?.Length ?? 0}");
                return;
            }

            boosterIcon.sprite = boosterIcons[iconIndex];
            Debug.Log($"[Booster] Set icon for {_type} (index: {iconIndex})");
        }

        private void UpdateQuantityText()
        {
            if (quantityText == null)
            {
                Debug.LogWarning("[Booster] quantityText is null!");
                return;
            }

            quantityText.text = _quantity.ToString();
            Debug.Log($"[Booster] Set quantity to {_quantity}");
        }
    }
}