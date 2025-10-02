using Defines;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class AutoGenerateLevel : MonoBehaviour
    {
        [SerializeField] private int levelID;
        [Header("Json")]
        [SerializeField] private TextAsset levelConfig;
        [Header("Prefab")] 
        [SerializeField] private GameObject prefabNormalBox;
        [SerializeField] private GameObject prefabNormalLockedBox;
        [SerializeField] private GameObject prefabRowRange;
        [SerializeField] private GameObject prefabColumnRange;
        [SerializeField] private GameObject prefabSpecialBox;

        private LevelConfig configs;

        private void LoadConfigs()
        {
            Debug.Log("Load Config");
            configs = JsonUtility.FromJson<LevelConfig>(levelConfig.text);
            Debug.Log(configs.LevelStrategies.Length);
            Debug.Log(configs.LevelStrategies[0].Id);
            Debug.Log(configs.LevelStrategies[0].NormalBox);
            Debug.Log(configs.LevelStrategies[0].NormalLockedBox);
        }

        public void Generate()
        {
            LoadConfigs();
            var config = System.Array.Find(configs.LevelStrategies, x => x.Id == levelID);
           
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

            Debug.Log(
                $"Generated level {levelID} with {config.NormalBox} normal boxes, {config.NormalLockedBox} locked boxes, {config.RowRange} row boxes, {config.ColumnRange} column boxes, {config.SpecialBox} special boxes");
        }

        private void ClearChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(AutoGenerateLevel))]
    public class AutoGenerateLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AutoGenerateLevel script = (AutoGenerateLevel)target;
            if (GUILayout.Button("Generate Level"))
            {
                script.Generate();
            }
        }
    }
#endif
}