using System.IO;
using System.Linq;
// using nadena.dev.modular_avatar.core;
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
                    foreach (var component in modularObject.GetComponents<Component>().Where(c => c))
                    {
                        switch (component.GetType().FullName)
                        {
                            case "VRC.Core.PipelineManager":
                            case "VRC.SDK3.Avatars.Components.VRCAvatarDescriptor":
                            case "Silksprite.Modularizer.ModularizerConfig":
                                Object.DestroyImmediate(component);
                                break;
                        }
                    }

                    // if (definition.SetupMA)
                    // {
                    //     var animator = modularObject.GetComponentInChildren<Animator>();
                    //     if (animator && animator.isHuman)
                    //     {
                    //         animator.GetBoneTransform(HumanBodyBones.Hips)?.parent.gameObject.AddComponent<ModularAvatarMergeArmature>();
                    //     }
                    // }
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
                IgnoreNonEmpty(renderer);
            }
            else
            {
                IgnoreEmpty(renderer);
            }
        }

        protected abstract GameObject InstantiateModule(GameObject gameObject);
        protected abstract void IgnoreEmpty(Renderer renderer);
        protected abstract void IgnoreNonEmpty(Renderer renderer);

        static string BuildPrefabPath(string path, string moduleName) => Path.Combine(path, $"{moduleName}.prefab");
    }
}