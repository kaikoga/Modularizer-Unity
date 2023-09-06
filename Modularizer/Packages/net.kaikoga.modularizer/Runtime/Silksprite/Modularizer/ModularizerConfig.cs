using Silksprite.Modularizer.DataObjects;
using UnityEngine;

namespace Silksprite.Modularizer
{
    [AddComponentMenu("Modularizer/Modularizer Config")]
    public class ModularizerConfig : MonoBehaviour
    {
        public string exportDirectory = "Assets/ModularizerPrefabs";
        public Transform avatarRoot;
        public Renderer bodyRenderer;
        public RendererSet[] modules = {};
        public bool unpackPrefab;
        // public bool setupMA = true;
        
        public Renderer[] selectedRenderers = {};

        public Renderer[] AllRenderers => avatarRoot.GetComponentsInChildren<Renderer>(true);
    }
}