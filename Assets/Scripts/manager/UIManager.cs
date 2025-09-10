using UnityEngine;
using Defines;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace manager
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<UIManager>();
                }
                return _instance;
            }
        }
        [SerializeField] private RectTransform commonUI;
        [SerializeField] private RectTransform popupUI;
        private Dictionary<string, GameObject> _commonMap = new();

        private void Awake()
        {
            _instance = this;
            // load MainUI from Resources
            LoadEventRegister();
            LoadMainUI();
        }
        
        private void LoadMainUI()
        {
            var prefab = Resources.Load<GameObject>("UI/MainUI");
            var instance = Instantiate(prefab, commonUI.transform);
            _commonMap.Add("MainUI", instance); // lưu instance chứ không phải prefab
        }
        
        private void LoadNextStageUI()
        {
            var prefab = Resources.Load<GameObject>("UI/NextStageUI");
            var instance = Instantiate(prefab, commonUI.transform);
            _commonMap.Add("NextStageUI", instance);
        }
        
        private void LoadEventRegister()
        {
            EventManager.Instance.On(EventKey.StartGame, () =>
            {
                Destroy(_commonMap.GetValueOrDefault("MainUI"));
                _commonMap.Remove("MainUI");
            });
            
            EventManager.Instance.On(EventKey.NextStage, () =>
            {
                LoadNextStageUI();
                var ui = _commonMap.GetValueOrDefault("NextStageUI");
                ui.transform.getComponent<NextStageUI>().Show();
                UniTask.Delay(1000).ContinueWith(() =>
                {
                    Destroy(ui);
                    _commonMap.Remove("NextStageUI");
                });
            });
        }
    }
}