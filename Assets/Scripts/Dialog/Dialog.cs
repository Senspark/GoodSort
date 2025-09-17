using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Dialog
{
    public class Dialog<T> : MonoBehaviour where T : Dialog<T>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform trans;
        [SerializeField] private Image overlay;
        
        private readonly List<Action> _willShowActions = new();
        private readonly List<Action> _didShowActions = new();
        private readonly List<Action> _willHideActions = new();
        private readonly List<Action> _didHideActions = new();
        
        public Canvas Canvas { get; private set; }
        protected bool HadShowed { get; set; } = false;
        protected 

    }
}