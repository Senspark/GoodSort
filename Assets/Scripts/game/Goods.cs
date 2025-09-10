using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using manager;


namespace game
{
    public class Goods : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image spriteImg;
        
        
        // getter and setter for opacity 
        private bool _visible;
        public bool Visible
        {
            get => _visible;
            set
            {
                Color currentColor = spriteImg.color;
                _visible = value;
                spriteImg.color = new Color(currentColor.r, currentColor.g, currentColor.b, _visible ? 1 : 0.5f);
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
                spriteImg.sprite = item;
                spriteImg.SetNativeSize();
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
                spriteImg.color = isFront ? Color.white : Color.gray;
            }
        }

        public int Slot { get; set; }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Layer == 0)
            {
                if(GameManager.Instance.IsPicking()) return;
                GameManager.Instance.Pick(this, eventData.position);
                Visible = false;
            }
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