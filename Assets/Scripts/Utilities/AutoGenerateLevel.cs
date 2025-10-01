using UnityEngine;

namespace Utilities
{
    public class AutoGenerateLevel : MonoBehaviour
    {
        [SerializeField] private int levelID;
        [Header("Prefab")] 
        [SerializeField] private GameObject prefabNormalBox;
        [SerializeField] private GameObject prefabNormalLockedBox;
        [SerializeField] private GameObject prefabRowBox;
        [SerializeField] private GameObject prefabColumnBox;
        [SerializeField] private GameObject prefabSpecialBox;
    }
}