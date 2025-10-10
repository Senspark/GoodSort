using System;
using UnityEngine;

namespace Core
{
    public class DragObject2 : MonoBehaviour, IDragObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Settings")] // 
        [SerializeField]
        private bool isDraggable = true;

        [SerializeField] private bool returnToOriginal = true;

        public int Id { get; private set; }
        public Vector3 Position => transform.position;

        // State
        private Vector3 _originalPosition;
        private Color _originalColor;
        private Vector3 _originalScale;
        private int _originalSortingOrder;
        private bool _isBeingDragged = false;
        private Func<bool> _canBeDragged;

        private void Start()
        {
            // Store original values
            _originalPosition = transform.position;
            _originalColor = Color.white;
            _originalScale = transform.localScale;
            _originalSortingOrder = spriteRenderer.sortingOrder;
        }

        public void Init(int id, Func<bool> canBeDragged)
        {
            Id = id;
            _canBeDragged = canBeDragged;
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
            return isDraggable && !_isBeingDragged && isActiveAndEnabled && _canBeDragged();
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
        public bool ShouldReturnToOriginal() => returnToOriginal;
        public void SetDraggable(bool draggable) => isDraggable = draggable;
    }
}