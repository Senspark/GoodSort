using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public class EffectUtils
    {
        public static void BlinkOnPosition(Vector3 position, GameObject container)
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Effect/Flash_round_ellow").Then(async effect =>
            {
                // Set position
                effect.transform.position = position;
                effect.transform.SetParent(container.transform, false);

                // Get particle system component and play
                var particleSystem = effect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                Object.Destroy(effect);

            });
        }

        public static void FlyStarToUI(Vector3 worldPos, RectTransform uiTarget, Canvas canvas, GameObject starPrefab, float duration = 0.8f)
        {
            var star = Object.Instantiate(starPrefab, canvas.transform);
            var rectTransform = star.GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            var screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                null,
                out var startLocalPos
            );

            rectTransform.anchoredPosition = startLocalPos;

            rectTransform.DOMove(uiTarget.position, duration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Object.Destroy(star));
        }
    }
}