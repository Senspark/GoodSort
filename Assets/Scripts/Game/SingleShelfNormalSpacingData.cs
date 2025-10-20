using Core;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Shelf/SingleNormal")]
    public class SingleShelfNormalSpacingData : ScriptableObject, ISpacingData
    {
        [SerializeField] private Vector2 topLayerSlotsPositions = new(0, -0.5f);
        [SerializeField] private Vector2 secondLayerSlotsPositions = new(0, -0.3f);

        public Vector2 GetPosition(int layerId, int slotId)
        {
            return layerId == 0 ? topLayerSlotsPositions : secondLayerSlotsPositions;
        }
    }
}