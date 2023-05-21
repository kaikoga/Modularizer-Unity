using System.Linq;
using Silksprite.Modularizer.Tools;
using UnityEngine;

namespace Silksprite.Modularizer.Models
{
    public class ModularizeDefinition
    {
        public GameObject RootObject { get; private set; }
        public ModuleDefinition[] Modules { get; private set; }
        public string ExportPath { get; private set; }

        public static ModularizeDefinition Build(ModularizerConfig config)
        {
            if (!config.avatarRoot) return null;
            if (string.IsNullOrWhiteSpace(config.exportDirectory)) return null;

            var rootObject = config.avatarRoot.gameObject;
            return new ModularizeDefinition
            {
                RootObject = rootObject,
                ExportPath = config.exportDirectory,
                Modules = rootObject.GetComponentsInChildren<Renderer>(true).Select(skinnedMeshRenderer => new ModuleDefinition
                {
                    ModuleName = $"{rootObject.name}_{skinnedMeshRenderer.gameObject.name}",
                    IsBaseModule = config.bodyRenderer == skinnedMeshRenderer,
                    Paths = new[]
                    {
                        rootObject.transform.GetRelativePath(skinnedMeshRenderer.transform)
                    }
                }).ToArray()
            };
        }

    }
}