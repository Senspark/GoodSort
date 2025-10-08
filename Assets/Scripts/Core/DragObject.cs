using UnityEngine;

namespace Core
{
    public class DragObject : MonoBehaviour, IDragObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Settings")] [SerializeField] private bool isDraggable = true;
        [SerializeField] private bool returnToOriginal = true;
        [SerializeField] private string objectType = "default";
        
        public int Id { get; private set; }

        // State
        private Vector3 _originalPosition;
        private Color _originalColor;
        private Vector3 _originalScale;
        private int _originalSortingOrder;
        private DropZone _currentZone;
        private bool _isBeingDragged = false;

        private void Start()
        {
            // Store original values
            _originalPosition = transform.position;
            _originalColor = spriteRenderer.color;
            _originalScale = transform.localScale;
            _originalSortingOrder = spriteRenderer.sortingOrder;
        }

        public void Init(int id)
        {
            Id = id;
        }

        // Called by manager
        public bool ContainsPosition(Vector2 position)
        {
            var bounds = spriteRenderer.bounds;
            var testPos = new Vector3(position.x, position.y, bounds.center.z);
            return bounds.Contains(testPos);
        }

        public bool CanBeDragged()
        {
            return isDraggable && !_isBeingDragged;
        }

        public void OnStartDrag()
        {
            _isBeingDragged = true;
            _originalPosition = transform.position;

            // Bring to front
            spriteRenderer.sortingOrder = _originalSortingOrder + 100;
        }

        public void OnEndDrag()
        {
            _isBeingDragged = false;

            // Reset sorting order
            spriteRenderer.sortingOrder = _originalSortingOrder;
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public void ApplyDragVisuals(Color dragColor, Vector3 dragScale)
        {
            spriteRenderer.color = dragColor;
            transform.localScale = dragScale;
        }

        public void ResetVisuals()
        {
            spriteRenderer.color = _originalColor;
            transform.localScale = _originalScale;
        }

        public void ReturnToOriginalPosition()
        {
            transform.position = _originalPosition;
        }

        // Getters/Setters
        public Vector3 GetOriginalPosition() => _originalPosition;
        public DropZone GetCurrentZone() => _currentZone;
        public void SetCurrentZone(DropZone zone) => _currentZone = zone;
        public bool ShouldReturnToOriginal() => returnToOriginal;
        public string GetObjectType() => objectType;
        public void SetDraggable(bool draggable) => isDraggable = draggable;
    }
}