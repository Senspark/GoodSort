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

        public void Initialize(LevelUI levelUI, PuzzleLevelData levelData)
        {
            _levelUI = levelUI;
            _levelUI.SetMaxTime(levelData.TimeLimit);
        }
        
        public void Step(float dt)
        {
            _levelUI.Step(dt);
        }
    }
}