using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CollectionStar : MonoBehaviour
    {
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private Transform starParent;
        [SerializeField] private Transform endPosition;
        [SerializeField] private ParticleSystem starEffect;
        [SerializeField] private int variantX = 5;
        [SerializeField] private int variantY = 5;
        List<GameObject> _stars = new();
        private Tween _starReactToCollection;
        private Action _onStarCollected;

        private void Start()
        {
            starEffect.Stop();
        }
        public async void CollectionStars(Vector3 spawnLocation, int amount, Action onStarCollected = null)
        {
            starEffect.transform.position = endPosition.position;
            _onStarCollected = onStarCollected;
            for(var i = 0; i < _stars.Count; i++)
            {
                Destroy(_stars[i]);
            }
            _stars.Clear();
            List<UniTask> spawnStarTaskList = new();
            for(var i = 0; i < amount; i++)
            {
                var starInstance = Instantiate(starPrefab, starParent);
                var rectTransform = starInstance.GetComponent<RectTransform>();

                var randomOffsetX = Random.Range(-variantX, variantX);
                var randomOffsetY = Random.Range(-variantY, variantY);
        
                // Gán vị trí trong UI Space
                rectTransform.anchoredPosition = new Vector2(spawnLocation.x + randomOffsetX, spawnLocation.y + randomOffsetY);
                spawnStarTaskList.Add(
                    starInstance.transform
                        .DOPunchPosition(new Vector3(0, 2, 0), Random.Range(0, 0.3f))
                        .ToUniTask()
                );
                _stars.Add(starInstance);
                await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            }
            await UniTask.WhenAll(spawnStarTaskList);
            await MoveStarsTask();
        }
        
        private async UniTask MoveStarsTask()
        {
            List<UniTask> moveStarTask = new();
            for(var i = _stars.Count - 1; i >= 0; i--)
            {
                moveStarTask.Add(MoveStarTask(_stars[i]));
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
            await UniTask.WhenAll(moveStarTask);
            _onStarCollected?.Invoke();
        }

        private async UniTask MoveStarTask(GameObject star)
        {
            await star.transform.DOMove(endPosition.position, 1f)
                .SetEase(Ease.InCubic)
                .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            _stars.Remove(star);
            Destroy(star);

            ReactToCollectionStar().Forget();
        }

        private async UniTaskVoid ReactToCollectionStar()
        {
            _starReactToCollection?.Rewind();
            _starReactToCollection?.Kill();

            endPosition.localScale = Vector3.one;
            starEffect.Play();

            _starReactToCollection = endPosition
                .DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.1f)
                .SetEase(Ease.OutQuad)
                .SetTarget(endPosition);
            await _starReactToCollection.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
}