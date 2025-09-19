using manager.Interface;
using UnityEngine;

namespace Game
{
    public interface ILevelView
    {
        public void Load(LevelConfigBuilder level, IEventManager eventManager);
    }
    public class LevelView : MonoBehaviour, ILevelView
    {
        public void Load(LevelConfigBuilder level, IEventManager eventManager)
        {
            
        }
    }
}