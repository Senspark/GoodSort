using Defines;
using UnityEngine;

namespace Utilities
{
    public class AutoGenerateLevel : MonoBehaviour
    {
        [SerializeField] private int levelID;
        [Header("Prefab")] 
        [SerializeField] private GameObject prefabNormalBox;
        [SerializeField] private GameObject prefabNormalLockedBox;
        [SerializeField] private GameObject prefabRowRange;
        [SerializeField] private GameObject prefabColumnRange;
        [SerializeField] private GameObject prefabSpecialBox;

        private LevelConfig configs;

        private void LoadConfigs()
        {
            if (configs != null) return;
            var jsonFile = Resources.Load<TextAsset>("Config/level_config");
            configs = JsonUtility.FromJson<LevelConfig>(jsonFile.text);
        }
        
        public void Generate()
        {
            LoadConfigs();
            var config = System.Array.Find(configs.levelStrategies, x => x.Id == levelID);
            if (config == null) return;
            ClearChildren();
            
            // Normal box
            for (var i = 0; i < config.NormalBox - config.NormalLockedBox; i++)
            {
                Instantiate(prefabNormalBox, transform);
            }
            
            // Normal locked box
            for (var i = 0; i < config.NormalLockedBox; i++)
            {
                Instantiate(prefabNormalLockedBox, transform);
            }
            
            // Row box
            for (var i = 0; i < config.RowRange; i++)
            {
                Instantiate(prefabRowRange, transform);
            }
            
            // Column box
            for (var i = 0; i < config.ColumnRange; i++)
            {
                Instantiate(prefabColumnRange, transform);
            }
            
            // Special box
            for (var i = 0; i < config.SpecialBox; i++)
            {
                Instantiate(prefabSpecialBox, transform);
            }
            Debug.Log($"Generated level {levelID} with {config.NormalBox} normal boxes, {config.NormalLockedBox} locked boxes, {config.RowRange} row boxes, {config.ColumnRange} column boxes, {config.SpecialBox} special boxes");
        }
        
        private void ClearChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}