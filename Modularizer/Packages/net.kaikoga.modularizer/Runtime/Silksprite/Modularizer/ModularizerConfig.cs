using Silksprite.Modularizer.DataObjects;
using UnityEngine;

namespace Silksprite.Modularizer
{
    [AddComponentMenu("Nugumin/Nugumin Config")]
    public class ModularizerConfig : MonoBehaviour
    {
        public string exportDirectory = "Assets/Modular Prefabs";
        public Transform avatarRoot;
        public Renderer bodyRenderer;
        public RendererSet[] modules = {};
        public bool unpackPrefab;

        public Renderer[] selectedRenderers = {};

        public Renderer[] AllRenderers => avatarRoot.GetComponentsInChildren<Renderer>(true);
    }
}