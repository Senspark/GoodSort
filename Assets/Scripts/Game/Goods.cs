using System.Collections;
using UnityEngine;
using DG.Tweening;
using UI;
using UnityEngine.Serialization;


namespace Game
{
    public class Goods : MonoBehaviour
    {
        [FormerlySerializedAs("spriteImg")] public SpriteRenderer spriteIcon;
        private bool _dragging;
        private Vector3 _startPos;
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
                spriteIcon.sortingOrder = isFront ? 10 : 2;
                spriteIcon.color = isFront ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.9f);
            }
        }

        private int _slot;
        public int Slot
        {
            get => _slot;
            set
            {
                _startPos = new Vector3(value - 1, 0, 0);
                transform.localPosition = _startPos;
                _slot = value;
            }
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _startPos = transform.localPosition;
            StartCoroutine(PullToLowestZ());
        }
        
        private IEnumerator PullToLowestZ()
        {
            yield return new WaitForSeconds(0.2f);
            transform.localPosition = new Vector3(_startPos.x, _startPos.y, -90);
        }

        private void OnMouseDrag()
        {
            if (!_dragging) return;

            Vector3 mousePos = Input.mousePosition;
    
            // khoảng cách từ camera tới object hiện tại
            mousePos.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z); 

            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0; // giữ luôn object ở z=0 trong thế giới 2D
    
            transform.position = worldPos;
            GameScene.Instance.OnMoveGoods(new Vector2(worldPos.x, worldPos.y));
        }


        public void OnMouseDown()
        {
            Debug.Log("KHOA TRAN - OnMouseDown");
            if(_layer != 0) return;
            Debug.Log("KHOA TRAN - OnMouseDown " + _layer);
            _dragging = true;
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
            GameScene.Instance.OnPickGoods(this, new Vector2(worldPos.x, worldPos.y));
            
            // _shelve.Controller.OnPickGoods(this, GetMousePosition());
        }

        private void OnMouseUp()
        {
            Debug.Log("KHOA TRAN - OnMouseUp");
            _dragging = false;
            transform.localPosition = _startPos;
            // _shelve.Controller.OnDropGoods();
            GameScene.Instance.OnDropGoods();
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