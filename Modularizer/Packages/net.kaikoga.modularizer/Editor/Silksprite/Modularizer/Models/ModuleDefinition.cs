using UnityEngine;

namespace Silksprite.Modularizer.Models
{
    public class ModuleDefinition
    {
        public string ModuleName { get; internal set; }
        public bool IsBaseModule { get; internal set; }
        public Mesh[] Meshes { get; internal set; }
    }
}