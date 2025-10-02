using System.Linq;
using UnityEngine;

namespace Game
{
    public class VerticalLoop : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool topToBottom = true;
        [Header("Custom Boundary")] 
        public Vector2 boundary = new(3f, 10f);
        [Header("Spacing")] 
        [SerializeField] private float spacing = 2f;

        private Transform[] boxes;

        void Start()
        {
            boxes = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                boxes[i] = transform.GetChild(i);
            }

            ArrangeInitialPositions();
        }

        void Update()
        {
            var direction = topToBottom ? -1f : 1f;
            var moveDelta = speed * Time.deltaTime * direction;

            foreach (var box in boxes)
            {
                box.localPosition += new Vector3(0, moveDelta, 0);

                var halfHeight = boundary.y;

                if (topToBottom && box.localPosition.y < -halfHeight)
                {
                    var maxY = GetMaxY();
                    box.localPosition = new Vector3(box.localPosition.x, maxY + spacing, box.localPosition.z);
                }
                else if (!topToBottom && box.localPosition.y > halfHeight)
                {
                    var minY = GetMinY();
                    box.localPosition = new Vector3(box.localPosition.x, minY - spacing, box.localPosition.z);
                }
            }
        }

        private void ArrangeInitialPositions()
        {
            var direction = topToBottom ? 1f : -1f;
            for (var i = 0; i < boxes.Length; i++)
            {
                var posY = i * spacing * direction;
                boxes[i].localPosition = new Vector3(0, posY, 0);
            }
        }

        private float GetMaxY()
        {
            return boxes.Select(box => box.localPosition.y).Prepend(float.MinValue).Max();
        }

        private float GetMinY()
        {
            return boxes.Select(box => box.localPosition.y).Prepend(float.MaxValue).Min();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(boundary.x, boundary.y, 0));
        }
    }
}