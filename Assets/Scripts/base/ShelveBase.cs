using System;
using System.Collections.Generic;
using Constant;
using Core;
using manager;
using UnityEngine;
using Game;
using manager.Interface;
using Senspark;

public class ShelveBase : MonoBehaviour, IShelf
{
    private int[][] _cachedData;
    protected Queue<List<int>> _layerQueue = new();
    private bool _isTargetTouched = false;
    protected int id;
    public int Id { get => id; set => id = value; }
    
    public bool IsTargetTouched() => _isTargetTouched;

    protected virtual void Awake()
    {
        id = transform.GetSiblingIndex();
        ServiceLocator.Instance.Resolve<IEventManager>().AddListener(EventKey.PlaceGood, OnPlaceGood);
    }

    protected virtual void Start() {}

    protected virtual void OnDestroy()
    {
        ServiceLocator.Instance.Resolve<IEventManager>().RemoveListener(EventKey.PlaceGood, OnPlaceGood);
    }
    
    public void SetLayerQueue(Queue<List<int>> layerQueue)
    {
        _layerQueue = layerQueue;
        
        // Cached export Data
        var layers = layerQueue.ToArray();
        var result = new int[layers.Length][];

        for (var i = 0; i < layers.Length; i++)
        {
            result[i] = layers[i].ToArray();
        }

        _cachedData = result;
    }
    
    public virtual void LoadNextLayers(){}

    public virtual void Init() {}

    public virtual int GetSlot(Vector3 pos)
    {
        return -1;
    }

    protected virtual void OnPlaceGood()
    {
    }

    public virtual bool IsSlotOccupied(int slotIndex)
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
    
    public int[][] ExportData()
    {
        return _cachedData ?? Array.Empty<int[]>();
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.name == "HoldingGoods")
        {
            _isTargetTouched = true;
        }
    }
    
    protected void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.name == "HoldingGoods")
        {
            _isTargetTouched = false;
        }
    }
}
