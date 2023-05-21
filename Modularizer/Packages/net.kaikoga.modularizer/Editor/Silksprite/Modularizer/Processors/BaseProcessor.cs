using System.IO;
using System.Linq;
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
                foreach (var skinnedMeshRenderer in modularObject.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (module.Meshes.Contains(skinnedMeshRenderer.sharedMesh)) continue;
                    Ignore(skinnedMeshRenderer);
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

        void Ignore(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (skinnedMeshRenderer.transform.childCount > 0 || skinnedMeshRenderer.gameObject.GetComponents<Component>().Length > 2)
            {
                IgnoreNonIsolated(skinnedMeshRenderer);
            }
            else
            {
                IgnoreIsolated(skinnedMeshRenderer);
            }
        }

        protected abstract GameObject InstantiateModule(GameObject gameObject);
        protected abstract void IgnoreIsolated(SkinnedMeshRenderer skinnedMeshRenderer);
        protected abstract void IgnoreNonIsolated(SkinnedMeshRenderer skinnedMeshRenderer);

        static string BuildPrefabPath(string path, string moduleName) => Path.Combine(path, $"{moduleName}.prefab");
    }
}