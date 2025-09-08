using UnityEngine;

namespace game
{
    public class Goods : MonoBehaviour, Ibegi
    {
        [SerializeField] private Sprite item;
        [SerializeField] private GameObject prefab;
        
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer SpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }
                return _spriteRenderer;
            }
        }
        
        // getter and setter for opacity 
        private float _opacity;
        public float Opacity
        {
            get {return _opacity;}
            set
            {
                _opacity = value;
                SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, _opacity);
            }
        }
        
        // getter and setter for Id
        private int _id;
        public int Id
        {
            get {return _id;}
            set
            {
                _id = value;
                item = Resources.Load<Sprite>("item_" + _id);
                SpriteRenderer.sprite = item;
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
                // if isFront, set color image is white, else gray
                SpriteRenderer.color = isFront ? Color.white : Color.gray;
            }
        }

        private int _slot;
        public int Slot
        {
            get => _slot;
            set => _slot = value;
        }
        
        private void Awake() {}
        
    }
}