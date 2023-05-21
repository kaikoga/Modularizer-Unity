using UnityEngine;
using UnityEngine.Serialization;

namespace Silksprite.Modularizer
{
    public class ModularizerConfig : MonoBehaviour
    {
        [FormerlySerializedAs("folder")] public string exportDirectory;
        public Transform avatarRoot;
        public bool unpackPrefab;
    }
}