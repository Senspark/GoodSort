using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class AnimateScene : MonoBehaviour
    {
        public async void StartAnimate(Transform parent)
        {
            var cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
            await AnimateBox(parent.Cast<Transform>().ToArray(), cancellationTokenOnDestroy);
        }

        private async UniTask AnimateBox(Transform[] objects, CancellationToken cancellationToken)
        {
            // Scale to 0 first
            foreach (var obj in objects)
            {
                obj.gameObject.SetActive(true);
                obj.localScale = Vector3.zero;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: cancellationToken);
            
            Array.Sort(objects, (a,b) => a.position.y.CompareTo(b.position.y));
            var tasks = new List<UniTask>(objects.Length);
            var currentDelay = 0f;
            const float delay = 0.15f;
            for(var i = 0; i < objects.Length; i++)
            {
                if (i > 0 && Math.Abs(objects[i].position.y - objects[i - 1].position.y) > 0.01f)
                {
                    currentDelay += delay;
                }
                tasks.Add(AnimateDelay(objects[i], cancellationToken, currentDelay));
            }
            await UniTask.WhenAll(tasks);
        }

        private async UniTask AnimateDelay(Transform box, CancellationToken cancellationToken, float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            _ = box.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WithCancellation(cancellationToken);
        }
    }
}