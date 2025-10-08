using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DropZone : MonoBehaviour, IDropZone
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Zone Settings")] //
        [SerializeField]
        private bool isActive = true;

        [SerializeField] private int maxObjects = 1;
        [SerializeField] private bool snapToCenter = true;

        [Header("Custom Bounds (used when SpriteRenderer is null)")] [SerializeField]
        private Vector2 customSize = new Vector2(1f, 1f);

        private List<IDragObject> _containedObjects;

        private void Start()
        {
            _containedObjects = new List<IDragObject>();
        }

        // Called by manager
        public bool ContainsPosition(Vector2 position)
        {
            if (!isActive) return false;

            var bounds = GetBounds();
            return bounds.Contains(position);
        }

        public bool CanAcceptObject(IDragObject dragObject)
        {
            if (!isActive) return false;
            return _containedObjects.Count < maxObjects;
        }

        public void AddObject(IDragObject dragObject)
        {
            if (!_containedObjects.Contains(dragObject))
            {
                _containedObjects.Add(dragObject);
            }
        }

        public void RemoveObject(IDragObject dragObject)
        {
            _containedObjects.Remove(dragObject);
        }

        public void ClearAllObjects()
        {
            _containedObjects.Clear();
        }

        // Getters
        public Vector3 GetSnapPosition(int slot)
        {
            var offset = new Vector3(slot, 0, 0);
            return transform.position + offset;
        }

        public bool ShouldSnapToCenter() => snapToCenter;
        public void SetActive(bool active) => isActive = active;

        // Helper method to get bounds
        private Bounds GetBounds()
        {
            if (spriteRenderer)
            {
                return spriteRenderer.bounds;
            }

            // Use custom bounds centered at transform position
            var bounds = new Bounds(transform.position, customSize);
            return bounds;
        }

        // Debug visualization
        private void OnDrawGizmos()
        {
            Gizmos.color = isActive ? Color.green : Color.red;

            var bounds = GetBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}