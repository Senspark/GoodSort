using System.Linq;
using UnityEngine;

namespace Game
{
    public class HorizontalLoop : MonoBehaviour
    {
        [Header("Movement Settings")] public float speed = 2f;
        public bool moveRight = true;
        public float offsetX = 0.2f;

        [Header("Custom Boundary")] public Vector2 boundary = new Vector2(10f, 3f); // giống cc.Size

        private Transform[] children;

        void Start()
        {
            int count = transform.childCount;
            children = new Transform[count];
            for (int i = 0; i < count; i++)
                children[i] = transform.GetChild(i);
        }

        void Update()
        {
            float dir = moveRight ? 1f : -1f;
            Vector3 move = new Vector3(dir * speed * Time.deltaTime, 0, 0);

            foreach (var child in children)
            {
                child.position += move;

                var sr = child.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                float halfWidth = sr.bounds.size.x / 2f;

                // kiểm tra biên dựa trên boundary custom
                float left = transform.position.x - boundary.x / 2f;
                float right = transform.position.x + boundary.x / 2f;

                if (moveRight)
                {
                    if (child.position.x - halfWidth > right)
                    {
                        float minX = GetMinChildX();
                        child.position = new Vector3(minX - sr.bounds.size.x - offsetX, child.position.y,
                            child.position.z);
                    }
                }
                else
                {
                    if (child.position.x + halfWidth < left)
                    {
                        float maxX = GetMaxChildX();
                        child.position = new Vector3(maxX + sr.bounds.size.x + offsetX, child.position.y,
                            child.position.z);
                    }
                }
            }
        }

        float GetMinChildX()
        {
            float min = float.MaxValue;
            foreach (var child in children)
                if (child.position.x < min)
                    min = child.position.x;
            return min;
        }

        float GetMaxChildX()
        {
            float max = float.MinValue;
            foreach (var child in children)
                if (child.position.x > max)
                    max = child.position.x;
            return max;
        }

        // Vẽ khung boundary trên Scene view
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(boundary.x, boundary.y, 0));
        }
    }
}