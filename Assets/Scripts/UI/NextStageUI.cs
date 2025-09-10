using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;

public class NextStageUI : MonoBehaviour
{
    [SerializeField] private RectTransform gc1Trans;
    [SerializeField] private RectTransform gc2Trans;
    [SerializeField] private RectTransform logo;

    private int gc0Height = 0;
    private int gc1Height = 0;

    private void Awake()
    {
        gc0Height = (int)gc1Trans.rect.height;
        gc1Height = (int)gc2Trans.rect.height;
    }

    public async Task FadeIn()
    {
        gameObject.SetActive(true);
        logo.localScale = Vector3.zero;
        await logo.DOScale(Vector3.one, 0.5f)
            .SetDelay(0.2f)
            .SetEase(Ease.OutBack)
            .AsyncWaitForCompletion();
        
        gc1Trans.sizeDelta = new Vector2(gc1Trans.rect.width, 0f);
        gc1Trans.DOSizeDelta(new Vector2(gc1Trans.rect.width, gc0Height), 0.2f);
        
        gc2Trans.sizeDelta = new Vector2(gc2Trans.rect.width, 0f);
        await gc2Trans.DOSizeDelta(new Vector2(gc2Trans.rect.width, gc1Height), 1f)
            .SetEase(Ease.OutCubic)
            .AsyncWaitForCompletion();
    }

    public async Task FadeOut()
    {
        logo.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        
        gc1Trans.DOSizeDelta(new Vector2(gc1Trans.rect.width, 0f), 0.2f)
            .SetDelay(0.8f);
        
        await gc2Trans.DOSizeDelta(new Vector2(gc2Trans.rect.width, 0f), 1f)
            .SetEase(Ease.OutCubic).AsyncWaitForCompletion();
        
        gameObject.SetActive(false);
    }
}
