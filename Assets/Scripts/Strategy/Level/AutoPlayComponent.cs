using System;
using System.Collections.Generic;
using System.Linq;
using Engine.ShelfPuzzle;
using Game;
using manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Strategy.Level
{
    public class AutoPlayComponent : MonoBehaviour
    {
        public static List<ShelfPuzzleNode> Solution;
        public LevelView levelView;

        private void Update()
        {
            if (Solution == null)
            {
                return;
            }

            var moves = Solution.Select(e => e.LastMove).ToList();
            CleanLogger.Log(JsonConvert.SerializeObject(moves));
            AutoPlay.Play(levelView, Solution);
            Solution = null;
        }
    }
}