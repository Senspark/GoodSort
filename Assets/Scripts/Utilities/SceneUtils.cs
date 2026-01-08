using manager.Interface;
using Senspark;

namespace Utilities
{
    public class SceneUtils
    {
        public static void LoadScene(string sceneName)
        {
            var sceneLoader = ServiceLocator.Instance.Resolve<ISceneLoader>();
            sceneLoader.LoadScene(sceneName);
        }
    }
}