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
        
        [Header("Star")]
        [SerializeField] private TMP_InputField edbStar;
        [SerializeField] private Button btnSaveStar;
        
        private void Start()
        {
            btnSaveLevel.onClick.AddListener(OnSaveLevel);
            btnSaveCoin.onClick.AddListener(OnSaveCoin);
            btnSaveStar.onClick.AddListener(OnSaveStar);
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
        
        private void OnSaveStar()
        {
            var star = int.Parse(edbStar.text);
            var storeManager = ServiceLocator.Instance.Resolve<IStoreManager>();
            storeManager.AddStars(star);
        }
        
        private void OnDestroy()
        {
            btnSaveLevel.onClick.RemoveListener(OnSaveLevel);
            btnSaveCoin.onClick.RemoveListener(OnSaveCoin);
            btnSaveStar.onClick.RemoveListener(OnSaveStar);
        }
    }
}