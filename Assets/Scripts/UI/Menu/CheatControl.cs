using manager.Interface;
using Senspark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class CheatControl : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private TMP_InputField edbLevel;
        [SerializeField] private Button btnSaveLevel;
        
        [Header("Coin")]
        [SerializeField] private TMP_InputField edbCoin;
        [SerializeField] private Button btnSaveCoin;
        
        private void Start()
        {
            btnSaveLevel.onClick.AddListener(OnSaveLevel);
            btnSaveCoin.onClick.AddListener(OnSaveCoin);
        }

        private void OnSaveLevel()
        {
            var level = int.Parse(edbLevel.text);
            var dataManager = ServiceLocator.Instance.Resolve<IDataManager>();
            dataManager.Set("CurrentLevel", level);
        }
        
        private void OnSaveCoin()
        {
            var coin = int.Parse(edbCoin.text);
            var storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();
            storeManager.AddCoins(coin);
        }
    }
}