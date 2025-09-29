using System.Collections;
using UnityEngine;
using DG.Tweening;
using UI;
using UnityEngine.Serialization;


namespace Game
{
    public class Goods : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer spriteIcon;
        private bool _dragging;
        private Vector3 _startPos;
      
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

                if (_layer == 0)
                {
                    spriteIcon.sortingLayerName = "Layer0";
                    spriteIcon.color = new Color(1, 1, 1, 1);
                }
                else if (_layer == 1)
                {
                    spriteIcon.sortingLayerName = "Layer1";
                    spriteIcon.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                }
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
            var dropZone = transform.GetComponent<DropZone>();
            if (dropZone)
            {
                dropZone.Free();
            }
            var seq = DOTween.Sequence();
            seq.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.15f));
            seq.Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.15f));
            seq.AppendCallback(() =>
            {
                // if (Layer == 0)
                // {
                //     Vector3 worldPos = transform.position;
                //     worldPos.y -= 20;
                // }
                Destroy(gameObject);
            });
        }
        
    }
}