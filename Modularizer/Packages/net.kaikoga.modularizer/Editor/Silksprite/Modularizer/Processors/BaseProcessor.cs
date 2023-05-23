using System.IO;
using Silksprite.Modularizer.Models;
using Silksprite.Modularizer.Tools;
using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer.Processors
{
    public abstract class BaseProcessor
    {
        public void Process(ModularizeDefinition definition)
        {
            var rootObject = definition.RootObject;

            foreach (var module in definition.Modules)
            {
                var modularObject = InstantiateModule(rootObject);
                modularObject.name = module.ModuleName;
                foreach (var renderer in modularObject.GetComponentsInChildren<Renderer>(true))
                {
                    if (module.ModularObjectContains(modularObject, renderer)) continue;
                    Ignore(renderer);
                }

                if (!module.IsBaseModule)
                {
                    foreach (var component in modularObject.GetComponents<Component>())
                    {
                        switch (component.GetType().FullName)
                        {
                            case "VRC.Core.PipelineManager":
                            case "VRC.SDK3.Avatars.Components.VRCAvatarDescriptor":
                                Object.DestroyImmediate(component);
                                break;
                        }
                    }
                }

                var assetPath = BuildPrefabPath(definition.ExportPath, module.ModuleName);
                ModularizerTools.EnsureDirectory(assetPath);
                PrefabUtility.SaveAsPrefabAsset(modularObject, assetPath);

                Object.DestroyImmediate(modularObject);
            }
        }

        void Ignore(Renderer renderer)
        {
            if (renderer.transform.childCount > 0 || renderer.gameObject.GetComponents<Component>().Length > 2)
            {
                IgnoreNonIsolated(renderer);
            }
            else
            {
                IgnoreIsolated(renderer);
            }
        }

        protected abstract GameObject InstantiateModule(GameObject gameObject);
        protected abstract void IgnoreIsolated(Renderer renderer);
        protected abstract void IgnoreNonIsolated(Renderer renderer);

        static string BuildPrefabPath(string path, string moduleName) => Path.Combine(path, $"{moduleName}.prefab");
    }
}