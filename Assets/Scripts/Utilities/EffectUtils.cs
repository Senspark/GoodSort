using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Effect;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public class EffectUtils
    {
        public static void Blink(Vector3 position)
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Effect/MagicPillarBlastYellow").Then(effect =>
            {
                effect.transform.position = position;
                effect.transform.SetParent(null);
                var ps = effect.GetComponent<ParticleSystem>();
                ps?.Play();
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

        public static async UniTask FlyMultipleStarsToUI(
            Vector3 worldPos,
            RectTransform uiTarget,
            Canvas canvas,
            GameObject starPrefab,
            int starCount = 5,
            Action onAllComplete = null)
        {
            const int maxDelay = 150;
            for (var i = 0; i < starCount; i++)
            {
                var randomOffset = UnityEngine.Random.insideUnitCircle * 1f;
                var randomStartPos = worldPos + new Vector3(randomOffset.x, randomOffset.y, 0);

                UniTask.Void(async () =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(UnityEngine.Random.Range(80, maxDelay)));
                    FlyStarToUI(randomStartPos, uiTarget, canvas, starPrefab);
                });
            }

            var totalWaitTimeMS = maxDelay + (int)(1f * 500);
            await UniTask.Delay(totalWaitTimeMS);

            onAllComplete?.Invoke();
        }

        public static void ShowComboText(Vector3 worldPos, Canvas canvas, int combo)
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Effect/Text_Combo").Then(comboPrefab =>
            {
                var comboObj = Object.Instantiate(comboPrefab, canvas.transform);
                var rectTransform = comboObj.GetComponent<RectTransform>();
                var comboVFX = comboObj.GetComponent<ComboVFX>();

                // var screenPos = Camera.main.WorldToScreenPoint(worldPos);
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //     canvas.transform as RectTransform,
                //     screenPos,
                //     null,
                //     out var localPos
                // );

                rectTransform.anchoredPosition = worldPos;

                comboVFX.SetComboVFX(GetComboColor(combo))
                    .SetText($"x{combo}")
                    .Play();
                return;

                ComboVFXType GetComboColor(int c)
                {
                    if (c >= 21)
                    {
                        return ComboVFXType.Gradient;
                    }

                    var position = ((c - 1) % 5) + 1;

                    return position switch
                    {
                        1 => ComboVFXType.Pink,    // combo 1, 6, 11, 16
                        2 => ComboVFXType.Orange,  // combo 2, 7, 12, 17
                        3 => ComboVFXType.Green,   // combo 3, 8, 13, 18
                        4 => ComboVFXType.Blue,    // combo 4, 9, 14, 19
                        5 => ComboVFXType.Violet,  // combo 5, 10, 15, 20
                        _ => ComboVFXType.Blue     // fallback
                    };
                }
            });
        }
    }
}