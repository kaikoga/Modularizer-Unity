using UnityEngine;

namespace Silksprite.Modularizer.Processors
{
    public class UnpackedPrefabProcessor : BaseProcessor
    {
        protected override GameObject InstantiateModule(GameObject gameObject) => Object.Instantiate(gameObject);

        protected override void IgnoreIsolated(Renderer renderer) => Object.DestroyImmediate(renderer.gameObject);

        protected override void IgnoreNonIsolated(Renderer renderer) => Object.DestroyImmediate(renderer);
    }
}