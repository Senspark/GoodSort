using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using manager.Interface;
using Senspark;
using Tutorial;
using UnityEngine;

namespace manager
{
    public class TutorialManager : ITutorialManager
    {
        private readonly IDataManager _dataManager;
        private const string SaveKey = nameof(TutorialManager);
        private TutorialData _tutorialData;
        private ITutorial _activeTutorial;
        
        public UniTask<bool> Initialize()
        {
            LoadData();
            return UniTask.FromResult(true);
        }
        
        // Constructor, inject data manager
        public TutorialManager(IDataManager dataManager)
        {
            _dataManager = dataManager;
        }

       
        public bool IsTutorialFinished(TutorialType tutorialType)
        {
            return _tutorialData.Data.GetValueOrDefault($"Tutorial_{tutorialType}", false);
        }

        public void FinishTutorial(TutorialType tutorialType)
        {
            _tutorialData.Data[$"Tutorial_{tutorialType}"] = true;
            SaveData();
        }
        
        // public bool CanInteract(string objectName)
        // {
        //     if (_activeTutorial == null) return true;
        //     return _activeTutorial.GetCurrentTargetName() == objectName;
        // }
        
        #region Data Persistence
        private void LoadData()
        {
            try
            {
                var raw = _dataManager.Get(SaveKey, "{}");
                var data = JsonUtility.FromJson<TutorialData>(raw);
                _tutorialData = data;
            }
            catch (Exception)
            {
                _tutorialData = new TutorialData();
            }
        }
        private void SaveData()
        {
            var json = JsonUtility.ToJson(_tutorialData);
            _dataManager.Set(SaveKey, json);
        }
        #endregion
        
        [Serializable]
        private class TutorialData
        {
            public Dictionary<string, bool> Data = new();
        }
    }
}