using UnityEngine;

namespace Core
{
    public class DropZone2 : MonoBehaviour, IDropZone
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float detectionRadius = 2.0f;

        [Header("Zone Settings")] //
        [SerializeField]
        private bool isActive = true;

        [SerializeField] private int maxObjects = 1;
        [SerializeField] private bool snapToCenter = true;

        [Header("Custom Bounds (used when SpriteRenderer is null)")] // 
        [SerializeField]
        private Vector2 customSize = new(1f, 1f);
        
        public int ShelfId { get; private set; }
        public int SlotId { get; private set; }

        public void Init(int shelfId, int slotId)
        {
            ShelfId = shelfId;
            SlotId = slotId;
        }

        // Called by manager
        public bool ContainsPosition(Vector2 position)
        {
            if (!isActive) return false;

            var bounds = GetBounds();
            if (bounds.Contains(position)) return true;

            var distance = Vector2.Distance(transform.position, position);
            return distance < detectionRadius;
        }

        // Getters
        public Vector3 GetSnapPosition(int slot)
        {
            var offset = new Vector3(slot, 0, 0);
            return transform.position + offset;
        }

        public float GetDetectionDistance(Vector2 position)
        {
            var distance = Vector2.Distance(transform.position, position);
            return distance;
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