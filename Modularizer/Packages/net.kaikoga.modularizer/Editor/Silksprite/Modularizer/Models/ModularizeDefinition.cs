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
        // public bool SetupMA { get; private set; }

        public static ModularizeDefinition Build(ModularizerConfig config)
        {
            if (!config.avatarRoot) return null;
            if (string.IsNullOrWhiteSpace(config.exportDirectory)) return null;

            var rootObject = config.avatarRoot.gameObject;
            return new ModularizeDefinition
            {
                RootObject = rootObject,
                ExportPath = config.exportDirectory,
                Modules = config.modules
                    .Select(module =>
                    {
                        return new ModuleDefinition
                        {
                            ModuleName = module.moduleName,
                            IsBaseModule = module.isBaseModule,
                            Paths = module.renderers.Select(renderer => rootObject.transform.GetRelativePath(renderer.transform)).ToArray()
                        };
                    }).ToArray(),
                // SetupMA = config.setupMA
            };
        }
    }

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
