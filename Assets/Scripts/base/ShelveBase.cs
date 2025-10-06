using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Core;
using Engine.ShelfPuzzle;
using UnityEngine;
using Game;
using manager.Interface;
using Senspark;

public abstract class ShelveBase : MonoBehaviour, IShelf
{
    private ShelfPuzzleInputData _input;
    protected Queue<List<int>> _layerQueue = new();
    private bool _isTargetTouched = false;
    public int Id { get; private set; }

    public bool IsTargetTouched() => _isTargetTouched;
    protected ShelfItem[] Items;
    protected IEventManager EventManager;

    protected virtual void Awake()
    {
        EventManager = ServiceLocator.Instance.Resolve<IEventManager>(); 
        EventManager.AddListener(EventKey.PlaceGood, OnPlaceGood);
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnDestroy()
    {
        EventManager.RemoveListener(EventKey.PlaceGood, OnPlaceGood);
    }

    public void SetLayerQueue(int shelfId, ShelfPuzzleInputData input)
    {
        Id = shelfId;
        _input = input;
        _layerQueue = new Queue<List<int>>();
        foreach (var layer in input.Data)
        {
            _layerQueue.Enqueue(layer.ToList());
        }
    }

    public virtual void LoadNextLayers()
    {
    }

    public virtual void Init()
    {
    }

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

    public virtual bool IsAllSlotOccupied()
    {
        return false;
    }

    public virtual bool IsEmpty()
    {
        return false;
    }

    public virtual ShelfItem CreateGoods(int goodsId, int slotId, int layer)
    {
        return null;
    }

    public virtual void PlaceGoods(int goodsId, int slotId)
    {
    }

    public virtual void Clear()
    {
    }

    public int[][] ExportData()
    {
        return _input != null ? _input.Data : Array.Empty<int[]>();
    }

    public ShelfItem FindItem(int id)
    {
        return Items.FirstOrDefault(e => e.Goods.Id == id);
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "HoldingGoods")
        {
            _isTargetTouched = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "HoldingGoods")
        {
            _isTargetTouched = false;
        }
    }
}