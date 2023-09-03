using Silksprite.Modularizer.DataObjects;
using UnityEngine;

namespace Silksprite.Modularizer
{
    public class ModularizerConfig : MonoBehaviour
    {
        public string exportDirectory;
        public Transform avatarRoot;
        public Renderer bodyRenderer;
        public RendererSet[] modules;
        public bool unpackPrefab;
        // public bool setupMA = true;
        
        public Renderer[] selectedRenderers;
    }
}