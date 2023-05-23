using System.Linq;
using Silksprite.Modularizer.Tools;
using UnityEngine;

namespace Silksprite.Modularizer.Models
{
    public class ModuleDefinition
    {
        public string ModuleName { get; internal set; }
        public bool IsBaseModule { get; internal set; }
        public string[] Paths { get; internal set; }

        public bool ModularObjectContains(GameObject modularObject, Renderer renderer)
        {
            return Paths.Contains(modularObject.transform.GetRelativePath(renderer.transform));
        }
    }
}