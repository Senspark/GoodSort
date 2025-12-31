using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Defines;
using Dialog.Controller;
using Senspark;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialog
{
    public class CompleteLevelDialog : Dialog<CompleteLevelDialog>
    {
        [SerializeField] private RectTransform pointer;
        [SerializeField] private RectTransform left;
        [SerializeField] private RectTransform right;
        [SerializeField] private ResourceBar coinBar;
        [SerializeField] private ClaimButton claimAdsRewardButton;
        [SerializeField] private ClaimButton claimRewardButton;
            
        [SerializeField] private GameObject coinPrefab;
        private ICompleteLevelDialogController _controller;
        private float _t;
        private bool _clickedClaim;
        private bool _isSlidingBonusBar;
        private const int BaseCoin = 10;

        private void Start()
        {
            IgnoreOutsideClick = true;
        }

        public override void Show(Canvas canvas)
        {
            OnWillShow(() =>
            {
                _controller.PlayEffect(AudioEnum.LevelComplete);
                _isSlidingBonusBar = true;
                
                claimRewardButton.SetMode(ClaimButton.Mode.Normal);
                claimRewardButton.Setup(BaseCoin, OnClaimReward);
                
                claimAdsRewardButton.SetMode(ClaimButton.Mode.WithAds, 2);
                claimAdsRewardButton.Setup(BaseCoin, OnClaimAdsReward);

                coinBar.Value = 0;
            });
            OnDidShow(() =>
            {
                // stop background music
                ServiceLocator.Instance.Resolve<IAudioManager>().StopMusic();
            });
            base.Show(canvas);
        }

        private async void OnClaimReward()
        {
            if (_clickedClaim) return;
            _controller.PlayEffect(AudioEnum.ClaimComplete);
            _clickedClaim = true;
            _isSlidingBonusBar = false;
            _ = ShowEffectReward(claimRewardButton.transform as RectTransform, coinBar.transform as RectTransform, BaseCoin);
            coinBar.AnimateTo(BaseCoin);
            _controller.AddCoins(BaseCoin); // ✅ Đổi từ AddStar → AddCoins
            _controller.BackToMenuScene();
        }
        
        private void OnClaimAdsReward()
        {
            if (_clickedClaim) return;
            _controller.PlayEffect(AudioEnum.ClaimComplete);
            _clickedClaim = true;
            _isSlidingBonusBar = false;
            var multiplier = CalculateMultiplier();
            claimAdsRewardButton.SetMode(ClaimButton.Mode.WithAds, multiplier);
            _ = ShowEffectReward(claimAdsRewardButton.transform as RectTransform, coinBar.transform as RectTransform, BaseCoin);
            coinBar.AnimateTo(BaseCoin * multiplier);
            _controller.AddCoins(BaseCoin * multiplier); // ✅ Đổi từ AddStar → AddCoins
            _controller.BackToMenuScene();
        }

        private int CalculateMultiplier()
        {
            var xPoint = pointer.anchoredPosition.x;
            if (Math.Abs(xPoint) < 85f) return 5;
            if (Math.Abs(xPoint) < 180f && Math.Abs(xPoint) >= 85f ) return 3;
            return 2;
        }
        
        void Update()
        {
            if (!_isSlidingBonusBar) return;
            _t += Time.deltaTime * 2f;
            var sinValue = (Mathf.Sin(_t) + 1f) * 0.5f; // dao động từ 0 → 1 → 0
            pointer.anchoredPosition = Vector2.Lerp(left.anchoredPosition, right.anchoredPosition, sinValue);
        }
        
        // Effect helper
        private GameObject SpawnCoin(Vector3 position)
        {
            var coin = Instantiate(coinPrefab, position, Quaternion.identity);
            coin.transform.SetParent(transform);
            return coin;
        }
        
        private async UniTask ShowEffectReward(RectTransform from, RectTransform to, int count)
        {
            var startPos = from.position;
            var endPos = to.position;

            var coins = new List<GameObject>(count);
            var tasks = new List<UniTask>(count);

            for (var i = 0; i < count; i++)
            {
                var offset = Random.insideUnitCircle * Random.Range(15f, 30f);
                var coin = SpawnCoin(startPos + (Vector3)offset);
                coins.Add(coin);
            }

            tasks.AddRange(Enumerable.Select(coins, coin => MoveStarAsync(coin, endPos)));

            await UniTask.WhenAll(tasks);
        }

        private async UniTask MoveStarAsync(GameObject star, Vector3 targetPos)
        {
            var startPos = star.transform.position;
            var midOffset = new Vector3(Random.Range(-100f, 100f), Random.Range(50f, 150f), 0f);

            var delay = Random.Range(0f, 0.1f);
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            var duration = Random.Range(0.6f, 0.9f);
            var tick = 0f;

            while (tick < 1f)
            {
                tick += Time.deltaTime / duration;
                var mid = Vector3.Lerp(startPos, targetPos, tick);
                mid += Vector3.Lerp(midOffset, Vector3.zero, tick); 
                star.transform.position = mid;
                await UniTask.Yield();
            }

            star.transform.position = targetPos;
            Destroy(star);
        }

    }
}