using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public class EffectUtils
    {
        public static void BlinkOnPosition(Vector3 position, GameObject container)
        {
            _ = PrefabUtils.LoadPrefab("Prefabs/Effect/Flash_round_ellow").Then(async effect =>
            {
                // Set position
                effect.transform.position = position;
                effect.transform.SetParent(container.transform, false);

                // Get particle system component and play
                var particleSystem = effect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                Object.Destroy(effect);

            });
        }
    }
}