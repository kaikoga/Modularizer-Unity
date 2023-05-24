using UnityEngine;
using UnityEngine.Serialization;

namespace Silksprite.Modularizer
{
    public class ModularizerConfig : MonoBehaviour
    {
        public string exportDirectory;
        public Transform avatarRoot;
        public Renderer bodyRenderer;
        public Renderer[] renderers;
        public bool unpackPrefab;
        public bool setupMA = true;
    }
}