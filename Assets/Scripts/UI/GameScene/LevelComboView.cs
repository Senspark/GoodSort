using System;
using Defines;
using Senspark;
using Strategy.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LevelComboView : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI scoreText;

        private IAudioManager _audioManager;

        private ComboData _comboData;

        private int _lastCombo;

        private void Awake()
        {
            _audioManager = ServiceLocator.Instance.Resolve<IAudioManager>();
        }

        private void Update()
        {
            if (_comboData == null) return;
            if (_lastCombo != _comboData.Combo)
            {
                _lastCombo = _comboData.Combo;
                if (_comboData.Combo > 0) PlaySoundCombo(_comboData.Combo);
            }
            UpdateDisplay();
        }

        public void SetComboData(ComboData comboData)
        {
            _comboData = comboData;
        }

        private void UpdateDisplay()
        {
            comboText.text = _comboData.Combo > 0 ? $"Combo x{_comboData.Combo}" : "";
            scoreText.text = $"{_comboData.Score}";
            progressBar.value = _comboData.GetProgress();
        }
        
        private void PlaySoundCombo(int combo)
        {
            Debug.Log("PlaySoundCombo: " + combo);
            _audioManager.PlaySound(AudioEnum.Match);
        }
    }
}