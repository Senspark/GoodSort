using System;
using UnityEngine;

namespace Game
{
    public class DropZone : MonoBehaviour
    {
        [Header("Drop Zone Settings")] public string acceptedItemType = "";
        public bool isOccupied;
        public Color highlightColor = Color.yellow;

        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        public event Action<DropZone, DragDrop> OnGoodsDropped;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }

        public bool CanAcceptItem(DragDrop item)
        {
            if (isOccupied) return false;

            // Add your own logic here (item type checking, etc.)
            return true;
        }

        public void AcceptItem(DragDrop item)
        {
            isOccupied = true;
            OnGoodsDropped?.Invoke(this, item);
            // Add additional logic for accepting items
        }

        public void Free()
        {
            isOccupied = false;
        }


        public Vector3 GetSnapPosition()
        {
            return transform.position;
        }

        public void SetHighlight(bool highlight)
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = highlight ? highlightColor : originalColor;
            }
        }
    }
}