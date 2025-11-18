using System;
using System.Collections.Generic;
using System.Linq;
using Defines;
using manager.Interface;
using Strategy.Level;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Game
{
    public enum LevelStatus
    {
        Invalid,
        Playing,
        Finished,
    }
    public class LevelView : MonoBehaviour 
    {
        private LevelUI _levelUI;
        private bool _loaded;
        public Action<Vector2> OnTopLayerCleared;

        public void Initialize(LevelUI levelUI, Canvas canvas, GameObject starPrefab)
        {
            _levelUI = levelUI;
            _loaded = true;
            _levelUI.SetMaxTime(900); // 10 phút
            OnTopLayerCleared = vector2 =>
            {
                var uiTarget = levelUI.GetComboViewTransform();
                EffectUtils.FlyStarToUI(vector2, uiTarget as RectTransform, canvas, starPrefab);
                _levelUI.AddScore();
            };
        }
        
        public void Step(float dt)
        {
            if(GetStatus() != LevelStatus.Playing) return;
            _levelUI.Step(dt);
        }
        
        public LevelStatus GetStatus()
        {
            if(!_loaded) return LevelStatus.Invalid;
            if(_levelUI.LevelTimeOut()) return LevelStatus.Finished;
            return LevelStatus.Playing;
        }
    }
}