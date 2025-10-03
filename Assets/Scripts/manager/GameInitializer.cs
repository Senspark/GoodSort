using System;
using Cysharp.Threading.Tasks;
using Defines;
using Newtonsoft.Json;
using UnityEngine;

namespace manager
{
    /**
     * Dùng để Initialize tạm không thông qua SplashScene
     */
    public static class MockGameInitializer
    {
        public static async UniTask Initialize()
        {
            try
            {
                var levelConfigFile = await Resources.LoadAsync<TextAsset>("Config/level_config");
                var goodsConfigFile = await Resources.LoadAsync<TextAsset>("Config/goods_config");
                var levelConfig = JsonConvert.DeserializeObject<LevelConfig>(levelConfigFile.ToString());
                var goodsConfig = JsonConvert.DeserializeObject<GoodsConfig[]>(goodsConfigFile.ToString());
                var initializeData = new ServiceInitializeData(levelConfig, goodsConfig);
                var initializer = new ServiceInitializer();
                await initializer.InitializeAllAsync(
                    initializeData,
                    null,
                    null
                );
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}