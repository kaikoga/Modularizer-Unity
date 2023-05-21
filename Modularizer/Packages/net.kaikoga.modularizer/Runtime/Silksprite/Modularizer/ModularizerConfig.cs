using UnityEngine;
using UnityEngine.Serialization;

namespace Silksprite.Modularizer
{
    public class ModularizerConfig : MonoBehaviour
    {
        public string exportDirectory;
        public Transform avatarRoot;
        public SkinnedMeshRenderer bodyRenderer;
        public SkinnedMeshRenderer[] renderers;
        public bool unpackPrefab;
    }
}