using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using manager;
using UnityEngine.Serialization;


namespace Game
{
    public class Goods : MonoBehaviour
    {
        [FormerlySerializedAs("spriteImg")] public SpriteRenderer spriteIcon;
        private bool _dragging;
        private Vector2 _startPos;
        private Vector2 _dragOffset;
        private Camera _mainCamera;
        private ShelveBase _shelve;
        public ShelveBase Shelve
        {
            get => _shelve;
            set => _shelve = value;
        }
        
        // getter and setter for opacity
        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                Color currentColor = spriteIcon.color;
                _visible = value;
                spriteIcon.color = new Color(currentColor.r, currentColor.g, currentColor.b, _visible ? 1 : 0);
            }
        }
        
        // getter and setter for Id
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                var item = Resources.Load<Sprite>("sprite/Items/Item" + _id);
                spriteIcon.sprite = item;
            }
        }

        private int _layer;
        public int Layer
        {
            get => _layer;
            set
            {
                _layer = value;
                var isFront = _layer == 0;
                spriteIcon.color = isFront ? Color.white : Color.gray;
            }
        }

        public int Slot { get; set; }

        private void Start()
        {
            _startPos = transform.position;
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (_dragging)
            {
                Vector2 mousePos = GetMousePosition();
                transform.position = mousePos;
                // _shelve.Controller.OnMoveGoods(mousePos);
                
            }
        }
        
        public void OnMouseDown()
        {
            _dragging = true;
            _startPos = transform.position;
            // _shelve.Controller.OnPickGoods(this, GetMousePosition());
        }

        void OnMouseUp()
        {
            _dragging = false;
            transform.position = _startPos;
            // _shelve.Controller.OnDropGoods();
        }

        private Vector2 GetMousePosition()
        {
            return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void Bounce(float delay = 0f)
        {
            // Reset scale
            transform.localScale = Vector3.one;

            // Sequence tween
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
            seq.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
            seq.Append(transform.DOScale(Vector3.one, 0.1f));
        }
        
        public void Remove()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.15f));
            seq.Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.15f));
            seq.AppendCallback(() =>
            {
                if (Layer == 0)
                {
                    Vector3 worldPos = transform.position;
                    worldPos.y -= 20;
                }
                Destroy(gameObject);
            });
        }
        
    }
}