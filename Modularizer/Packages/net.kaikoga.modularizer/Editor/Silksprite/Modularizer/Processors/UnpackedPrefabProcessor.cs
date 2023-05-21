using UnityEngine;

namespace Silksprite.Modularizer.Processors
{
    public class UnpackedPrefabProcessor : BaseProcessor
    {
        protected override GameObject InstantiateModule(GameObject gameObject) => Object.Instantiate(gameObject);

        protected override void IgnoreIsolated(SkinnedMeshRenderer skinnedMeshRenderer) => Object.DestroyImmediate(skinnedMeshRenderer.gameObject);

        protected override void IgnoreNonIsolated(SkinnedMeshRenderer skinnedMeshRenderer) => Object.DestroyImmediate(skinnedMeshRenderer);
    }
}