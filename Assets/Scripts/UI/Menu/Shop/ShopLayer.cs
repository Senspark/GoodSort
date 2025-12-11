using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Menu
{
    public class ShopLayer : MonoBehaviour
    {
        [SerializeField] private NestedScroll _nestedScroll;

        private const string SHOP_PREFAB_PATH = "Prefabs/Shop/";

        private async void Start()
        {
            await LoadShopItems();
        }

        /// <summary>
        /// Load all shop item prefabs and instantiate them into the content
        /// </summary>
        private async UniTask LoadShopItems()
        {
            RemoveAllChildren();
            string[] prefabNames = GetShopPrefabNames();
            foreach (var prefabName in prefabNames)
            {
                await LoadAndInstantiatePrefab(prefabName);
            }
        }

        /// <summary>
        /// Remove all children from the nested scroll view content
        /// </summary>
        private void RemoveAllChildren()
        {
            var content = _nestedScroll.content;
            var childCount = content.childCount;

            // Destroy all children
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Get list of shop prefab names
        /// </summary>
        private string[] GetShopPrefabNames()
        {
            return new[]
            {
                "Starter Pack",
                "Mini Pack",
                "Standard Pack",
                "Deluxe Pack",
                "Elite Pack",
                "Master Pack",
                "Super Pack",
                "Daily Special",
                "Revive Offer",
                "Remove Ads",
            };
        }

        /// <summary>
        /// Load prefab from Resources and instantiate it into content
        /// </summary>
        private async UniTask LoadAndInstantiatePrefab(string prefabName)
        {
            var fullPath = SHOP_PREFAB_PATH + prefabName;

            try
            {
                var request = Resources.LoadAsync<GameObject>(fullPath);
                await request;
                var prefab = request.asset as GameObject;
                if (prefab == null)
                {
                    Debug.LogError($"[ShopLayer] Failed to load prefab: {fullPath}");
                    return;
                }

                // Instantiate prefab
                var instance = Instantiate(prefab, _nestedScroll.content);
                instance.name = prefabName;

                // Resize pack to 90% screen width
                ResizePackToScreenWidth(instance);

                Debug.Log($"[ShopLayer] Loaded and instantiated: {prefabName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ShopLayer] Exception loading {prefabName}: {e.Message}");
            }
        }
        private void ResizePackToScreenWidth(GameObject pack, float widthPercentage = 0.9f)
        {
            var rectTransform = pack.GetComponent<RectTransform>();
            var contentWidth = _nestedScroll.content.rect.width;
            var packWidth = contentWidth * widthPercentage;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, packWidth);
        }

        public async void Reload()
        {
            await LoadShopItems();
        }
    }
}