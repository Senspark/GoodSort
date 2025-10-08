using UnityEngine;

namespace Core
{
    public interface IDropZone
    {
        void ClearAllObjects();
        bool CanAcceptObject(IDragObject obj);
        void AddObject(IDragObject obj);
        bool ShouldSnapToCenter();
        Vector3 GetSnapPosition(int slot);
        bool ContainsPosition(Vector2 position);
        void RemoveObject(IDragObject dragObject);
    }
}