using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Defines;
using Senspark;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Dialog
{
    public class Dialog<T> : MonoBehaviour where T : Dialog<T>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image overlay;
        
        private readonly List<Action> _willShowActions = new();
        private readonly List<Action> _didShowActions = new();
        private readonly List<Action> _willHideActions = new();
        private readonly List<Action> _didHideActions = new();
        
        public Canvas Canvas { get; private set; }
        protected bool HadShowed { get; set; } = false;
        protected bool IgnoreOutsideClick { get; set; } = false;
        protected bool UseActionsOnDestroy { get; set; } = false;
        
        private Sequence _showSequence;
        private Sequence _hideSequence;
        private bool _isHiding = false;
        private UniTaskCompletionSource<bool> _waitForDialogClosedTask;

        protected virtual void Awake()
        {
            SetupClickOutside();
            _waitForDialogClosedTask = new UniTaskCompletionSource<bool>();
        }

        protected bool IsHiding()
        {
            return _isHiding;
        }

        protected virtual void OnDestroy()
        {
            _showSequence.Kill();
            _hideSequence.Kill();
            // _waitForDialogClosedTask.SetResult(true);
            if (UseActionsOnDestroy)
            {
                _willHideActions.ForEach(action => action());
                _didHideActions.ForEach(action => action());
            }
        }
        
        public Dialog<T> OnWillShow(Action action)
        {
            Assert.IsNotNull(action);
            _willShowActions.Add(action);
            return this;
        }
        
        public Dialog<T> OnDidShow(Action action)
        {
            Assert.IsNotNull(action);
            _didShowActions.Add(action);
            return this;
        }
        
        public Dialog<T> OnWillHide(Action action)
        {
            Assert.IsNotNull(action);
            _willHideActions.Add(action);
            return this;
        }
        
        public Dialog<T> OnDidHide(Action action)
        {
            Assert.IsNotNull(action);
            _didHideActions.Add(action);
            return this;
        }

        public virtual void Show(Canvas canvas)
        {
            canvasGroup.alpha = 0;
            Canvas = canvas;
            transform.SetParent(canvas.transform, false);
            FadeToShow();
        }

        public void DelayShow(Canvas canvas, float delay)
        {
            canvasGroup.alpha = 0;
            Canvas = canvas;
            transform.SetParent(canvas.transform, false);
            FadeToShow(delay);
        }

        public async UniTask WaitForHide()
        {
            await _waitForDialogClosedTask.Task;
        }

        private void FadeToShow(float delay = 0f)
        {
            HadShowed = false;
            _willShowActions.ForEach(item => item?.Invoke());
            // _willHideActions.Clear();

            var fade = canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutBack);
            var sequence = DOTween.Sequence();
            if (delay > 0)
            {
                sequence.SetDelay(delay);
            }
            sequence.Append(fade);
            _showSequence = sequence.AppendCallback(() =>
                {
                    _didShowActions.ForEach(item => item?.Invoke());
                    // _didHideActions.Clear();
                    HadShowed = true;
                })
                .SetUpdate(UpdateType.Normal, true);
        }

        public void Hide()
        {
            if(_isHiding) return;
            _isHiding = true;
            _willHideActions.ForEach(item => item?.Invoke());
            // _willShowActions.Clear();
            var fade = canvasGroup.DOFade(0, 0.3f).SetEase(Ease.InOutSine);
            _hideSequence = DOTween.Sequence()
                .Append(fade)
                .AppendCallback(() =>
                {
                    _didHideActions.ForEach(item => item?.Invoke());
                    // _didShowActions.Clear();
                    ServiceLocator.Instance.Resolve<IAudioManager>().PlaySound(AudioEnum.CloseDialog, 1f);
                    _isHiding = false;
                    Destroy(gameObject);
                })
                .SetUpdate(UpdateType.Normal, true);
        }

        private void SetupClickOutside()
        {
            if(!overlay) return;
            var trigger = overlay.GetComponent<EventTrigger>();
            if(!trigger) trigger = overlay.gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) =>
            {
                if (IgnoreOutsideClick) return;
                Hide();
            });
            trigger.triggers.Add(entry);
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}