using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DropZone : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Zone Settings")] //
        [SerializeField]
        private bool isActive = true;

        [SerializeField] private int maxObjects = 1;
        [SerializeField] private string[] acceptedTypes;
        [SerializeField] private bool snapToCenter = true;

        [Header("Custom Bounds (used when SpriteRenderer is null)")]
        [SerializeField] private Vector2 customSize = new Vector2(1f, 1f);

        private List<DragObject> _containedObjects;

        private void Start()
        {
            _containedObjects = new List<DragObject>();
        }

        // Called by manager
        public bool ContainsPosition(Vector2 position)
        {
            if (!isActive) return false;

            var bounds = GetBounds();
            return bounds.Contains(position);
        }

        public bool CanAcceptObject(DragObject dragObject)
        {
            if (!isActive) return false;
            if (_containedObjects.Count >= maxObjects) return false;

            // Check type restrictions
            if (acceptedTypes != null && acceptedTypes.Length > 0)
            {
                var objType = dragObject.GetObjectType();
                var typeAccepted = false;

                foreach (var acceptedType in acceptedTypes)
                {
                    if (objType == acceptedType)
                    {
                        typeAccepted = true;
                        break;
                    }
                }

                if (!typeAccepted) return false;
            }

            return true;
        }

        public void AddObject(DragObject dragObject)
        {
            if (!_containedObjects.Contains(dragObject))
            {
                _containedObjects.Add(dragObject);
            }
        }

        public void RemoveObject(DragObject dragObject)
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
        public int GetSortingOrder() => spriteRenderer ? spriteRenderer.sortingOrder : 0;
        public List<DragObject> GetContainedObjects() => new(_containedObjects);
        public int GetObjectCount() => _containedObjects.Count;
        public bool IsActive() => isActive;
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