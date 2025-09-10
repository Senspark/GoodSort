using manager;
using UnityEngine;
using game;
using Defines;

public class ShelveBase : MonoBehaviour
{
    protected int id;
    public int Id { get => id; set => id = value; }

    protected virtual void Awake()
    {
        id = transform.GetSiblingIndex();
        EventManager.Instance.On(EventKey.PlaceGood, OnPlaceGood);
    }

    protected virtual void Start() {}

    protected void OnDestroy()
    {
        EventManager.Instance.Off(EventKey.PlaceGood, OnPlaceGood);
    }

    public virtual void Init(int bounceDelay = 0) {}

    public virtual int GetSlot(Vector3 pos)
    {
        return -1;
    }

    protected virtual void OnPlaceGood()
    {
    }

    public bool IsSlotOccupied(int slotIndex)
    {
        return false;
    }
    
    public bool IsAllSlotOccupied()
    {
        return false;
    }
    
    public virtual bool IsEmpty()
    {
        return false;
    }

    public virtual Goods CreateGoods(int goodsId, int slotId, int layer)
    {
        return null;
    }
    
    public virtual void PlaceGoods(int goodsId, int slotId)
    {
        
    }

    public void removeGoods(Goods goods)
    {
        
    }
    
    public virtual void Clear()
    {
        
    }
}
