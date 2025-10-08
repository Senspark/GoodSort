using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Central manager controls all drag & drop operations
    /// </summary>
    public class DragDropGameManager : MonoBehaviour
    {
        [SerializeField] private float dragZOffset = -1f;
        /* Object chứa các Drag & Drop */
        [SerializeField] private Transform container;

        [Header("Visual Settings")] [SerializeField]
        private bool enableVisualFeedback = true;

        [SerializeField] private Color dragColor = new(1f, 1f, 1f, 0.8f);
        [SerializeField] private Vector3 dragScale = new(1.1f, 1.1f, 1f);

        // Registered objects
        private List<DragObject> _dragObjects;
        private List<DropZoneData> _dropZones;

        // Current drag state
        private DragObject _currentDraggingObject;
        private Vector3 _dragOffset;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (!_mainCamera)
            {
                Debug.LogError("Main Camera not found!");
            }
            
            _dragObjects = new List<DragObject>();
            _dropZones = new List<DropZoneData>();
        }

        private void Update()
        {
            HandleMouseInput();
            UpdateDragging();
        }
        
        #region PUBLIC_METHODS

        // Public registration methods
        public void RegisterDragObject(DragObject dragObject)
        {
            if (!_dragObjects.Contains(dragObject))
            {
                _dragObjects.Add(dragObject);
            }
        }

        public void UnregisterDragObject(DragObject dragObject)
        {
            _dragObjects.Remove(dragObject);
        }

        public void RegisterDropZone(DropZoneData dropZone)
        {
            UnregisterDropZone(dropZone.Zone);
            _dropZones.Add(dropZone);
        }

        public void UnregisterDropZone(DropZone dropZone)
        {
            var index = _dropZones.FindIndex(e => e.Zone == dropZone);
            if (index >= 0)
            {
                _dropZones.RemoveAt(index);
            }
        }

        // Public utility methods
        public bool IsDragging()
        {
            return _currentDraggingObject != null;
        }

        public DragObject GetCurrentDraggingObject()
        {
            return _currentDraggingObject;
        }

        public void ResetAllObjects()
        {
            foreach (var obj in _dragObjects)
            {
                obj.ReturnToOriginalPosition();
            }

            foreach (var zone in _dropZones)
            {
                zone.Zone.ClearAllObjects();
            }
        }

        #endregion

        #region PRIVATE_METHODS

        private void HandleMouseInput()
        {
            // Mouse down - start drag
            if (Input.GetMouseButtonDown(0) && !_currentDraggingObject)
            {
                TryStartDrag();
            }

            // Mouse up - end drag
            if (Input.GetMouseButtonUp(0) && _currentDraggingObject)
            {
                EndDrag();
            }
        }

        private void TryStartDrag()
        {
            var mouseWorldPos = GetMouseWorldPosition();
            var objectUnderMouse = GetDragObjectAtPosition(mouseWorldPos);

            if (objectUnderMouse && objectUnderMouse.CanBeDragged())
            {
                StartDrag(objectUnderMouse);
            }
        }

        private void StartDrag(DragObject dragObject)
        {
            _currentDraggingObject = dragObject;

            // Calculate offset
            var mousePos = GetMouseWorldPosition();
            _dragOffset = dragObject.transform.position - mousePos;

            dragObject.OnStartDrag();

            if (enableVisualFeedback)
            {
                dragObject.ApplyDragVisuals(dragColor, dragScale);
            }
        }

        private void UpdateDragging()
        {
            if (!_currentDraggingObject) return;

            var mousePos = GetMouseWorldPosition();
            var newPosition = mousePos + _dragOffset;
            newPosition.z = _currentDraggingObject.GetOriginalPosition().z + dragZOffset;

            _currentDraggingObject.UpdatePosition(newPosition);
        }

        private void EndDrag()
        {
            if (!_currentDraggingObject) return;

            // Find drop zone at current position
            var dropPosition = _currentDraggingObject.transform.position;
            var targetZoneData = GetDropZoneAtPosition(dropPosition);
            var targetZone = targetZoneData.Zone;

            var successfulDrop = false;

            if (targetZone && targetZone.CanAcceptObject(_currentDraggingObject))
            {
                var previousZone = _currentDraggingObject.GetCurrentZone();
                if (previousZone)
                {
                    previousZone.RemoveObject(_currentDraggingObject);
                }

                // Add to new zone
                targetZone.AddObject(_currentDraggingObject);
                _currentDraggingObject.SetCurrentZone(targetZone);

                // Snap position if needed
                if (targetZone.ShouldSnapToCenter())
                {
                    _currentDraggingObject.UpdatePosition(targetZone.GetSnapPosition(0));
                }

                targetZoneData.OnDropped(_currentDraggingObject.Id);
                

                successfulDrop = true;
            }

            // Handle unsuccessful drop
            if (!successfulDrop && _currentDraggingObject.ShouldReturnToOriginal())
            {
                _currentDraggingObject.ReturnToOriginalPosition();
            }

            _currentDraggingObject.ResetVisuals();
            _currentDraggingObject.OnEndDrag();
            _currentDraggingObject = null;
        }

        // Helper methods
        private Vector3 GetMouseWorldPosition()
        {
            var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }

        private DragObject GetDragObjectAtPosition(Vector2 position)
        {
            foreach (var dragObj in _dragObjects)
            {
                if (dragObj.isActiveAndEnabled && dragObj.ContainsPosition(position))
                {
                    return dragObj;
                }
            }

            return null;
        }

        private DropZoneData GetDropZoneAtPosition(Vector2 position)
        {
            // Check zones in reverse order (top to bottom)
            for (var i = _dropZones.Count - 1; i >= 0; i--)
            {
                if (_dropZones[i].Zone.ContainsPosition(position))
                {
                    return _dropZones[i];
                }
            }

            return null;
        }

        #endregion
    }

    public class DropZoneData
    {
        public DropZone Zone;
        public Action<int> OnDropped;
    }
}