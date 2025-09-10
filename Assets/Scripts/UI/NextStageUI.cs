using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Defines;
using manager;

public class NextStageUI : MonoBehaviour
{
    [SerializeField] private RectTransform gc1Trans;
    [SerializeField] private RectTransform gc2Trans;
    [SerializeField] private RectTransform logo;
    [SerializeField] private Button nextButton;

    private int _gc0Height;
    private int _gc1Height;

    private void Awake()
    {
        _gc0Height = (int)gc1Trans.rect.height;
        _gc1Height = (int)gc2Trans.rect.height;
        nextButton.onClick.AddListener(() =>
        {
            EventManager.Instance.Emit(EventKey.NextStage);
        });
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        logo.localScale = Vector3.zero;
        gc1Trans.sizeDelta = new Vector2(gc1Trans.rect.width, 0f);
        gc2Trans.sizeDelta = new Vector2(gc2Trans.rect.width, 0f);
    
        logo.DOScale(Vector3.one, 0.5f)
            .SetDelay(0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                gc1Trans.sizeDelta = new Vector2(gc1Trans.rect.width, 0f);
                gc1Trans.DOSizeDelta(new Vector2(gc1Trans.rect.width, _gc0Height), 0.2f);
            
                gc2Trans.sizeDelta = new Vector2(gc2Trans.rect.width, 0f);
                gc2Trans.DOSizeDelta(new Vector2(gc2Trans.rect.width, _gc1Height), 1f)
                    .SetEase(Ease.OutCubic);
            });
    }

    public void FadeOut()
    {
        logo.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    
        gc1Trans.DOSizeDelta(new Vector2(gc1Trans.rect.width, 0f), 0.2f)
            .SetDelay(0.8f);
    
        gc2Trans.DOSizeDelta(new Vector2(gc2Trans.rect.width, 0f), 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
    }
}
