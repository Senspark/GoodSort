using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Effect;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public class EffectUtils
    {
        public static void BlinkOnPosition(Vector3 position, GameObject container)
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Effect/MagicPillarBlastYellow").Then(effect =>
            {
                UniTask.Void(async () =>
                {
                    effect.transform.SetParent(container.transform, true);
                    effect.transform.position = position;

                    var ps = effect.GetComponent<ParticleSystem>();
                    ps?.Play();

                    await UniTask.Delay(TimeSpan.FromSeconds(2f));
                    Object.Destroy(effect);
                });
            });
        }

        public static void FlyStarToUI(
            Vector3 worldPos,
            RectTransform uiTarget,
            Canvas canvas,
            GameObject starPrefab,
            float duration = 0.5f)
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

        public static void FlyStarToUI_ver2(
            Vector3 worldPos,
            Transform worldTarget,
            GameObject container,
            GameObject starPrefab,
            float duration = 0.5f)
        {
            var star = Object.Instantiate(starPrefab, container.transform);
            star.transform.position = worldPos;

            star.transform.DOMove(worldTarget.position, duration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Object.Destroy(star));
        }

        public static void FlyMultipleStarsToUI(
            Vector3 worldPos,
            RectTransform uiTarget,
            Canvas canvas,
            GameObject starPrefab,
            int starCount = 5,
            float spreadRadius = 1f,
            float duration = 1f)
        {
            for (int i = 0; i < starCount; i++)
            {
                var randomOffset = UnityEngine.Random.insideUnitCircle * spreadRadius;
                var randomStartPos = worldPos + new Vector3(randomOffset.x, randomOffset.y, 0);

                UniTask.Void(async () =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(UnityEngine.Random.Range(80, 150)));
                    FlyStarToUI(randomStartPos, uiTarget, canvas, starPrefab, duration);
                });
            }
        }

        public static void ShowComboText(Vector3 worldPos, Canvas canvas, string text, ComboVFXType type)
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Effect/Text_Combo").Then(comboPrefab =>
            {
                var comboObj = Object.Instantiate(comboPrefab, canvas.transform);
                var rectTransform = comboObj.GetComponent<RectTransform>();
                var comboVFX = comboObj.GetComponent<ComboVFX>();

                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                var screenPos = Camera.main.WorldToScreenPoint(worldPos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPos,
                    null,
                    out var localPos
                );

                rectTransform.anchoredPosition = localPos;

                comboVFX.SetComboVFX(type)
                    .SetText(text)
                    .Play();
            });
        }
    }
}