namespace Core
{
    public interface IDragDropGameManager
    {
        void RegisterDragObject(IDragObject dragObject);
        void UnregisterDragObject(int dragId);
        void RegisterDropZone(DropZoneData dropZone);
        void UnregisterDropZone(IDropZone dropZone);
        void ResetAllObjects();
        void RemoveAll();
        void Pause();
        void Unpause();
    }
}