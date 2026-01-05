using System;
using DG.Tweening;
using manager.Interface;
using Senspark;
using TMPro;
using UnityEngine;

namespace UI
{
    
    public class ResourceBar : MonoBehaviour
    {
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private TMP_Text label;
        [SerializeField] private bool isDisplayMaxValue;
        
        private int _value;
        private int _maxValue = -1; // -1 means +infinity
        // Action PlusResourceCallback;
        public event Action _plusResourceAction;

        // <summary>
        // Setter, Getter for _value
        // </summary>
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateDisplay();
            }
        }
        
        // <summary>
        // Setter, Getter for _maxValue
        // </summary>
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                UpdateDisplay();
            }
        }
        
        public void SetPlusResourceAction(Action action)
        {
            _plusResourceAction = action;
        }

        private void Awake()
        {
            // UpdateDisplay();
            // take value from store manager
            var storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();
            switch (resourceType)
            {
                case ResourceType.Coins:
                    _value = storeManager.GetCoins();
                    break;
                case ResourceType.Stars:
                    _value = storeManager.GetTotalStars();
                    break;
                case ResourceType.Lives:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (isDisplayMaxValue)
            {
                label.text = $"{_value}/{_maxValue}";
            }
            else
            {
                label.text = $"{_value}";
            }
        }

        public void AnimateTo(int targetValue, float duration = 1f)
        {
            DOTween.To(() => _value, x => _value = x, targetValue, duration)
                .OnUpdate(UpdateDisplay);
        }

        public void PressedPlusButton()
        {
            _plusResourceAction?.Invoke();
        }
    }
}