using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class ResourceBar : MonoBehaviour
    {
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