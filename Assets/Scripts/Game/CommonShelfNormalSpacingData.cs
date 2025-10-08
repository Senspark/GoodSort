using Core;
using manager;
using UnityEngine;

namespace Game
{
    // [CreateAssetMenu(menuName = "Game/Common/ShelfNormal")]
    public class CommonShelfNormalSpacingData : ScriptableObject, ISpacingData
    {
        [SerializeField] private Vector2[] topLayerSlotsPositions = new[]
        {
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1),
        };

        [SerializeField] private Vector2[] secondLayerSlotsPositions = new[]
        {
            new Vector2(-0.8f, -0.8f),
            new Vector2(0, -0.8f),
            new Vector2(0.8f, -0.8f),
        };

        public Vector2 GetPosition(int layerId, int slotId)
        {
            if (slotId < 0 || slotId >= 3)
            {
                CleanLogger.Error($"Sai Slot Id: {slotId}");
                return Vector2.zero;
            }

            if (layerId < 0 || layerId > 1)
            {
                CleanLogger.Error($"Sai Layer Id: {layerId}");
                return Vector2.zero;
            }

            return layerId == 0 ? topLayerSlotsPositions[slotId] : secondLayerSlotsPositions[slotId];
        }
    }
}