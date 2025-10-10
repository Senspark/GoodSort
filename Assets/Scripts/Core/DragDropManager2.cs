using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Central manager controls all drag & drop operations
    /// </summary>
    public class DragDropManager2 : MonoBehaviour, IDragDropManager
    {
        [SerializeField] private float dragZOffset = -1f;

        /* Object chứa các Drag & Drop */
        [SerializeField] private Transform container;

        [Header("Visual Settings")] //
        [SerializeField]
        private bool enableVisualFeedback = true;

        [SerializeField] private Color dragColor = new(1f, 1f, 1f, 0.8f);
        [SerializeField] private Vector3 dragScale = new(1.1f, 1.1f, 1f);

        private Func<IDropZone, bool> _canAcceptDropIntoFunc;

        // Registered objects
        private List<IDragObject> _dragObjects;
        private List<DropZoneData> _dropZones;

        // Current drag state
        private IDragObject _currentDraggingObject;
        private Vector3 _dragOffset;
        private Camera _mainCamera;
        private bool _pause = false;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (!_mainCamera)
            {
                Debug.LogError("Main Camera not found!");
            }

            _dragObjects = new List<IDragObject>();
            _dropZones = new List<DropZoneData>();
        }

        private void Update()
        {
            if (_pause) return;
            HandleMouseInput();
            UpdateDragging();
        }

        #region PUBLIC_METHODS

        public void Init(Func<IDropZone, bool> canAcceptDropIntoFunc)
        {
            _canAcceptDropIntoFunc = canAcceptDropIntoFunc;
        }

        // Public registration methods
        public void RegisterDragObject(IDragObject dragObject)
        {
            if (!_dragObjects.Contains(dragObject))
            {
                _dragObjects.Add(dragObject);
            }
        }

        public void UnregisterDragObject(int dragId)
        {
            var drag = _dragObjects.FirstOrDefault(d => d.Id == dragId);
            if (drag == null) return;
            _dragObjects.Remove(drag);
        }

        public void RegisterDropZone(DropZoneData dropZone)
        {
            UnregisterDropZone(dropZone.Zone);
            _dropZones.Add(dropZone);
        }

        public void UnregisterDropZone(IDropZone dropZone)
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

        public IDragObject GetCurrentDraggingObject()
        {
            return _currentDraggingObject;
        }

        public void ResetAllObjects()
        {
            foreach (var obj in _dragObjects)
            {
                obj.ReturnToOriginalPosition();
            }
        }

        public void RemoveAll()
        {
            _currentDraggingObject = null;
            _dragObjects.Clear();
            _dropZones.Clear();
        }

        public void Pause()
        {
            _pause = true;
        }

        public void Unpause()
        {
            _pause = false;
        }

        public void ManualDropInto(IDragObject dragObject, IDropZone dropZone)
        {
            var targetZoneData = _dropZones.Find(e => e.Zone == dropZone);
            DropInto(dragObject, targetZoneData);
        }

        #endregion

        #region PRIVATE_METHODS

        private void HandleMouseInput()
        {
            // Mouse down - start drag
            if (Input.GetMouseButtonDown(0) && _currentDraggingObject == null)
            {
                TryStartDrag();
            }

            // Mouse up - end drag
            if (Input.GetMouseButtonUp(0) && _currentDraggingObject != null)
            {
                EndDrag();
            }
        }

        private void TryStartDrag()
        {
            var mouseWorldPos = GetMouseWorldPosition();
            var objectUnderMouse = GetDragObjectAtPosition(mouseWorldPos);

            if (objectUnderMouse != null)
            {
                StartDrag(objectUnderMouse);
            }
        }

        private void StartDrag(IDragObject dragObject)
        {
            _currentDraggingObject = dragObject;

            // Calculate offset
            var mousePos = GetMouseWorldPosition();
            _dragOffset = dragObject.Position - mousePos;

            dragObject.OnStartDrag();

            if (enableVisualFeedback)
            {
                dragObject.ApplyDragVisuals(dragColor, dragScale);
            }
        }

        private void UpdateDragging()
        {
            if (_currentDraggingObject == null) return;

            var mousePos = GetMouseWorldPosition();
            var newPosition = mousePos + _dragOffset;
            newPosition.z = _currentDraggingObject.GetOriginalPosition().z + dragZOffset;

            _currentDraggingObject.UpdatePosition(newPosition);
        }

        private void EndDrag()
        {
            if (_currentDraggingObject == null) return;
            var dragObject = _currentDraggingObject;

            // Find drop zone at current position
            var dropPosition = dragObject.Position;
            var targetZoneData = GetDropZoneAtPosition(dropPosition);

            var successfulDrop = false;

            if (targetZoneData != null)
            {
                successfulDrop = DropInto(dragObject, targetZoneData);
            }

            // Handle unsuccessful drop
            if (!successfulDrop && dragObject.ShouldReturnToOriginal())
            {
                dragObject.ReturnToOriginalPosition();
            }

            dragObject.ResetVisuals();
            dragObject.OnEndDrag();
            _currentDraggingObject = null;
        }

        private bool DropInto(IDragObject dragObject, DropZoneData targetZoneData)
        {
            var targetZone = targetZoneData.Zone;

            if (targetZone != null && _canAcceptDropIntoFunc(targetZone))
            {
                // Snap position if needed
                if (targetZone.ShouldSnapToCenter())
                {
                    dragObject.UpdatePosition(targetZone.GetSnapPosition(0));
                }

                StartCoroutine(ScheduleCallback(targetZoneData, dragObject.Id));

                return true;
            }

            return false;
        }

        // Helper methods
        private Vector3 GetMouseWorldPosition()
        {
            var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }

        private IDragObject GetDragObjectAtPosition(Vector2 position)
        {
            foreach (var dragObj in _dragObjects)
            {
                if (dragObj.CanBeDragged() && dragObj.ContainsPosition(position))
                {
                    return dragObj;
                }
            }

            return null;
        }

        [CanBeNull]
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

        private static IEnumerator ScheduleCallback(DropZoneData dropZone, int dragId)
        {
            yield return null; // next frame
            dropZone.OnDropped(dragId);
        }

        #endregion
    }

    public class DropZoneData
    {
        public IDropZone Zone;
        public Action<int> OnDropped;
    }
}