using UnityEngine;

namespace Utilities
{
    [ExecuteAlways]
    public class SiblingSorter : MonoBehaviour
    {
        void LateUpdate()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var sr = child.GetComponent<SpriteRenderer>();
                if(sr == null) continue;
                sr.sortingOrder = i;
            }
        }
    }
}