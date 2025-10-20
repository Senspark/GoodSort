using System.Linq;
using UnityEngine;

namespace Game
{
    public class HorizontalLoop : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool rightToLeft = true;
        [Header("Custom Boundary")] 
        public Vector2 boundary = new(10f, 3f);
        [Header("Spacing")] 
        [SerializeField] private float spacing = 2f;

        private Transform[] boxes;

        void Start()
        {
            boxes = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
            {
                boxes[i] = transform.GetChild(i);
            }
            ArrangeInitialPositions();
        }

        void Update()
        {
            var direction = rightToLeft ? -1f : 1f;
            var moveDelta = speed * Time.deltaTime * direction;

            foreach (var box in boxes)
            {
                box.localPosition += new Vector3(moveDelta, 0, 0);

                var halfWidth = boundary.x;

                if (rightToLeft && box.localPosition.x < -halfWidth)
                {
                    var maxX = GetMaxX();
                    box.localPosition = new Vector3(maxX + spacing, box.localPosition.y, box.localPosition.z);
                }
                else if (!rightToLeft && box.localPosition.x > halfWidth)
                {
                    var minX = GetMinX();
                    box.localPosition = new Vector3(minX - spacing, box.localPosition.y, box.localPosition.z);
                }
            }
        }

        private void ArrangeInitialPositions()
        {
            var direction = rightToLeft ? 1f : -1f;
            for (var i = 0; i < boxes.Length; i++)
            {
                var posX = i * spacing * direction;
                boxes[i].localPosition = new Vector3(posX, 0, 0);
            }
        }

        private float GetMaxX()
        {
            return boxes.Select(box => box.localPosition.x).Prepend(float.MinValue).Max();
        }

        private float GetMinX()
        {
            return boxes.Select(box => box.localPosition.x).Prepend(float.MaxValue).Min();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(boundary.x, boundary.y, 0));
        }
    }
}