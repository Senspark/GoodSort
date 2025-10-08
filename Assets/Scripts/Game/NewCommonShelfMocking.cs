using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    /**
     * Shelf này chỉ có 3 Slot chứa hiển thị Item.
     * Nhưng chia ra làm 2 lớp: Top-Layer & Second-Layer
     * Top-Layer: có màu đầy đủ
     * Second-Layer: có màu nhạt hơn
     */
    [ExecuteInEditMode]
    public class NewCommonShelfMocking : MonoBehaviour
    {
        /**
         * Kích cỡ tối đa của item w ⨯ h: mặc đinh là 1 unit ⨯ 2 unit
         */
        [SerializeField] private Vector2 maxSize = new(1, 2);

        [SerializeField] private Vector2[] topLayerSlotsPositions;
        [SerializeField] private Vector2[] secondLayerSlotsPositions;

        private const int MaxSlot = 3;

        /**
         * Bấm nút này để tự sắp xếp lại các slot cho cân bằng
         */
        [Button]
        private void EditorRePosition()
        {
            var (topLayer, secondLayer) = GetLayers();
            RePosition(topLayer, topLayerSlotsPositions);
            RePosition(secondLayer, secondLayerSlotsPositions);
            return;

            void RePosition(Transform parent, Vector2[] positions)
            {
                for (var i = 0; i < MaxSlot; i++)
                {
                    var slot = parent.GetChild(i).GetComponent<SpriteRenderer>();
                    var position = positions[i];

                    // Canh kích thước tối đa để hiển thị cho đẹp
                    // Tự scale Pixel Per Unit sẽ hay hơn
                    /***
                    var spriteSize = slot.sprite.bounds.size;
                    var scaleX = maxSize.x / spriteSize.x;
                    var scaleY = maxSize.y / spriteSize.y;
                    var scale = Mathf.Min(scaleX, scaleY);
                    slot.transform.localScale = new Vector3(scale, scale, 1f);
                    **/

                    var bounds = slot.bounds;
                    var halfHeight = bounds.size.y / 2f;
                    slot.transform.position = new Vector2(position.x, position.y + halfHeight);
                }
            }
        }

        /**
         * Bấm Button này để tự nó setup trên Editor
         */
        [Button]
        private void EditorInit()
        {
            var (topLayer, secondLayer) = GetLayers();

            secondLayerSlotsPositions = ScanSlots(secondLayer);
            topLayerSlotsPositions = ScanSlots(topLayer);
            return;

            Vector2[] ScanSlots(Transform parent)
            {
                var slots = new Vector2[MaxSlot];
                for (var i = 0; i < MaxSlot; i++)
                {
                    var slot = parent.GetChild(i);
                    slots[i] = slot.position;
                }

                return slots;
            }
        }

        private (Transform, Transform) GetLayers()
        {
            if (transform.childCount != 2)
            {
                Debug.LogError("Cấu hình Prefab không đúng: Chỉ support 2 layer");
                return (null, null);
            }

            var secondLayer = transform.GetChild(0);
            var topLayer = transform.GetChild(1);
            return (topLayer, secondLayer);
        }
    }
}