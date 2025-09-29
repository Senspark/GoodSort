using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Game
{
    public class DragDrop : MonoBehaviour
    {
        [Header("Inventory Settings")] public LayerMask dropZoneLayer;

        public LayerMask goodsLayer;

        public float snapDistance = 0.5f;
        public bool returnToOriginIfInvalidDrop = true;

        private bool isDragging;
        private Vector3 originalPosition;
        private Transform originalParent;
        private DropZone currentDropZone;
        private Camera mainCamera;
        private Collider2D objectCollider;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        void Start()
        {
            objectCollider = GetComponent<Collider2D>();
            spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;
            originalPosition = transform.position;
            originalParent = transform.parent;
            originalColor = spriteRenderer.color;
        }

        void Update()
        {
            HandleDragInput();
        }

        // get set CurrentDropZone
        public DropZone CurrentZone { get; set; }

        private void HandleDragInput()
        {
            var mouseWorldPos = GetMouseWorldPosition();

            if (Input.GetMouseButtonDown(0))
            {
                if (IsMouseOverObject(mouseWorldPos))
                {
                    StartDragging();
                }
            }

            if (isDragging)
            {
                transform.position = mouseWorldPos;
                CheckDropZones();
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                StopDragging();
            }
        }

        private void StartDragging()
        {
            var goods = GetComponent<Goods>();
            if (goods.Layer != 0) return;
            isDragging = true;
            originalPosition = transform.position;

            // Visual feedback

            // Bring to front
            spriteRenderer.sortingOrder = 100;
        }

        private void StopDragging()
        {
            isDragging = false;
            var goods = GetComponent<Goods>();

            // Reset visual feedback
            // spriteRenderer.color = originalColor;
            spriteRenderer.sortingOrder = 0;

            if (currentDropZone && currentDropZone.CanAcceptItem(this))
            {
                // Valid drop
                DropIntoZone(currentDropZone);
            }
            else if (returnToOriginIfInvalidDrop)
            {
                // Invalid drop - return to origin
                ReturnToOrigin();
            }
        }

        private void CheckDropZones()
        {
            var dropZones = Physics2D.OverlapPointAll(transform.position, dropZoneLayer);

            // Reset previous drop zone highlight
            // if (currentDropZone != null)
            // {
            //     currentDropZone.SetHighlight(false);
            //     currentDropZone = null;
            // }

            // Find valid drop zone
            foreach (var dropZoneCollider in dropZones)
            {
                var dropZone = dropZoneCollider.GetComponent<DropZone>();
                if (dropZone && dropZone.CanAcceptItem(this))
                {
                    currentDropZone = dropZone;
                    dropZone.SetHighlight(true);
                    break;
                }
            }
        }

        private void DropIntoZone(DropZone dropZone)
        {
            transform.position = dropZone.GetSnapPosition();
            dropZone.AcceptItem(this);
            dropZone.SetHighlight(false);
        }

        private void ReturnToOrigin()
        {
            StartCoroutine(SmoothMoveToPosition(originalPosition, 0.3f));
        }

        private IEnumerator SmoothMoveToPosition(Vector3 targetPos, float duration)
        {
            var startPos = transform.position;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / duration;
                t = t * t * (3f - 2f * t); // Smooth step
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
        }

        private Vector3 GetMouseWorldPosition()
        {
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z);
            return mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }

        private bool IsMouseOverObject(Vector3 mouseWorldPos)
        {
            // Lấy tất cả collider thuộc layer goods tại vị trí chuột
            var hits = Physics2D.OverlapPointAll(mouseWorldPos, goodsLayer);
            if (hits.Length == 0) return false;

            Collider2D topCollider = null;
            var topLayerValue = int.MinValue;
            var topOrder = int.MinValue;

            foreach (var h in hits)
            {
                var sr = h.transform.GetChild(0).GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                var layerVal = SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
                var order = sr.sortingOrder;

                // chọn cái có layer cao hơn, hoặc cùng layer nhưng order cao hơn
                if (layerVal <= topLayerValue && (layerVal != topLayerValue || order <= topOrder)) continue;
                topCollider = h;
                topLayerValue = layerVal;
                topOrder = order;
            }

            // Debug xem Unity chọn gì
            // Debug.Log($"Topmost under mouse: {topCollider?.name}");

            // Chỉ cho phép drag nếu objectCollider chính là topmost
            return topCollider == objectCollider;
        }
    }
}