using Silksprite.Modularizer.DataObjects;
using UnityEngine;

namespace Silksprite.Modularizer
{
    [AddComponentMenu("NuguminTool/NuguminTool Config")]
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