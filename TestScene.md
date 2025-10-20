Dependency Flow

```
TestScene
    ├─→ DragDropGameManager (injected)
    ├─→ LevelCreator (created)
    │   └─→ Returns LevelData
    ├─→ LevelDataManager (created with LevelData)
    └─→ LevelAnimation (created with DataManager + DragDropManager)
        ├─→ LevelDataManager (passed in)
        └─→ DragDropGameManager (passed in)
    
    ShelfItemBasic
    ├─→ ShelfItemMeta (data)
    ├─→ DragObject (component)
    └─→ Callback to TestScene.OnItemDestroy
```