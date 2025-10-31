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
    public class LevelView : MonoBehaviour 
    {
        private LevelUI _levelUI;
        public Action<Vector2> OnTopLayerCleared;

        public void Initialize(LevelUI levelUI)
        {
            _levelUI = levelUI;
            _levelUI.SetMaxTime(600); // 10 phút
            OnTopLayerCleared = vector2 =>
            {
                _levelUI.AddScore();
            };
        }
        
        public void Step(float dt)
        {
            _levelUI.Step(dt);
        }
    }
}