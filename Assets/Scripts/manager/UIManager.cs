using System;
using UnityEngine;
using System.Collections.Generic;
using Constant;
using manager.Interface;
using Senspark;

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
            LoadUILayer();
        }

        private void Start()
        {
            _commonMap["MainUI"].SetActive(true);
        }

        private void LoadUILayer()
        {
            var mainUI = Resources.Load<GameObject>("UI/MainUI");
            var nextStageUI = Resources.Load<GameObject>("UI/NextStageUI");
            _commonMap.Add("MainUI", Instantiate(mainUI, commonUI.transform));
            _commonMap.Add("NextStageUI", Instantiate(nextStageUI, commonUI.transform));
            
            foreach (var ui in _commonMap)
            {
                ui.Value.SetActive(false);
            }
        }
        
        private void LoadEventRegister()
        {
            var eventManager = ServiceLocator.Instance.Resolve<IEventManager>();
            eventManager.AddListener(EventKey.StartGame, () =>
            {
                _commonMap["MainUI"].SetActive(false);
            });
            eventManager.AddListener(EventKey.OnStageComplete, () =>
            {
                var ui = _commonMap.GetValueOrDefault("NextStageUI");
                ui.transform.GetComponent<NextStageUI>().FadeIn();
            });
            eventManager.AddListener(EventKey.NextStage, () =>
            {
                var ui = _commonMap.GetValueOrDefault("NextStageUI");
                ui.transform.GetComponent<NextStageUI>().FadeOut();
            });
        }
    }
}