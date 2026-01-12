using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private RectTransform uiCanvas;
    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform scrollingDoor;
    private const float Duration = 0.5f;
    private float _referenceHeight;
    private Tween _currentTween;
    public static SceneTransition Instance;

    private void Awake()
    {
        // _referenceHeight = uiCanvas.rect.height;
        // bar.gameObject.SetActive(false);
        // scrollingDoor.sizeDelta = new Vector2(scrollingDoor.sizeDelta.x, 0);
        if (Instance == null)
        {
            Instance = this;
            _referenceHeight = uiCanvas.rect.height;
            bar.gameObject.SetActive(false);
            scrollingDoor.sizeDelta = new Vector2(scrollingDoor.sizeDelta.x, 0);
            DontDestroyOnLoad(gameObject); // Lệnh quan trọng nhất
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }

    public async UniTask FadeIn()
    {
        bar.gameObject.SetActive(true);

        scrollingDoor.sizeDelta = new Vector2(scrollingDoor.sizeDelta.x, 0);

        var tween = scrollingDoor.DOSizeDelta(new Vector2(scrollingDoor.sizeDelta.x, _referenceHeight), Duration)
            .SetEase(Ease.OutCubic);

        await UniTask.WaitUntil(() => !tween.IsActive() || !tween.IsPlaying());
    }

    public async UniTask FadeOut()
    {
        var tween = scrollingDoor.DOSizeDelta(new Vector2(scrollingDoor.sizeDelta.x, 0), Duration)
            .SetEase(Ease.InCubic);
        await UniTask.WaitUntil(() => !tween.IsActive() || !tween.IsPlaying());
        bar.gameObject.SetActive(false);
    }
}
