using manager;
using UnityEngine;
using game;

public class ShelveBase : MonoBehaviour
{
    protected int id;
    public int Id { get => id; set => id = value; }

    protected virtual void Awake()
    {
        id = transform.GetSiblingIndex();
        EventManager.on(EventKey.PLACE_GOOD, OnPlaceGood);
    }

    protected virtual void Start() {}

    protected void OnDestroy()
    {
        EventManager.on(EventKey.PLACE_GOOD, OnPlaceGood);
    }

    public void Init(int bounceDelay = 0) {}

    public int GetSlot(Vector3 pos)
    {
        return 0;
    }

    protected void OnPlaceGood() {}

    public bool IsSlotOccupied(int slotIndex)
    {
        return false;
    }
    
    public bool IsAllSlotOccupied()
    {
        return false;
    }
    
    public bool IsEmpty()
    {
        return false;
    }

    public void CreateGoods(int goodsId, int slotId, int layer)
    {
        
    }
    
    public void PlaceGoods(int goodsId, int slotId)
    {
        
    }

    public void removeGoods(Goods goods)
    {
        
    }
    
    public void clear()
    {
        
    }
}
