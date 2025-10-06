using System;
using System.Collections.Generic;
using System.Linq;
using Core;
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

    public static class GameExportData
    {
        /**
         * Export format như sau:
         * [
         *      [
         *          [slot_0, slot_1, slot_2],   // layer 0
         *          [slot_0, slot_1, slot_2],   // layer 1
         *          ...                         // các layer khác
         *      ],  // shelf 0
         *
         *      ... // các shelf khác
         * ]
         */
        public static void ExportData(List<IShelf> list)
        {
            CleanLogger.Log("Game Export Data:");
            foreach (var shelf in list)
            {
                var shelfExported = shelf.ExportData();
                var formatted = string.Join(", ", shelfExported.Select(e => $"[{string.Join(", ", e)}]"));
                CleanLogger.Log($"Shelf #{shelf.Id} = {formatted}");
            }

            CleanLogger.Log("==================");
        }
    }
}