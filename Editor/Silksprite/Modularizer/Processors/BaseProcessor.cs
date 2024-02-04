using System.Collections.Generic;
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

                foreach (var component in modularObject.GetComponents<Component>().Where(c => c))
                {
                    switch (component.GetType().FullName)
                    {
                        case "VRC.Core.PipelineManager":
                        case "VRC.SDK3.Avatars.Components.VRCAvatarDescriptor":
                            if (!module.IsBaseModule) Object.DestroyImmediate(component);
                            break;
                        case "Silksprite.Modularizer.ModularizerConfig":
                            Object.DestroyImmediate(component);
                            break;
                    }
                }

#if UNITY_2022_2_OR_NEWER
                for (var i = 0; i < 10; i++)
                {
                    if (!GC(modularObject)) break;
                }
#endif

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

        bool GC(GameObject modularObject)
        {
            var references = new HashSet<Transform>();
            var componentReferences = new HashSet<Component>();

            foreach (var child in modularObject.GetComponentsInChildren<Transform>())
            {
                var components = child.GetComponents<Component>()
                    .Where(c=> !(c is Transform))
                    .ToArray();

                foreach (var component in components)
                {
                    var isVrcPhysBone = false;
                    // prefilter components
                    switch (component.GetType().FullName)
                    {
                        case "VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone":
                            isVrcPhysBone = true;
                            if (child.childCount == 0) continue;
                            break;
                        case "VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider":
                            // require reference
                            continue;
                    }

                    // register components
                    references.Add(component.transform);
                    componentReferences.Add(component);

                    // prefilter references
                    if (component is SkinnedMeshRenderer smr)
                    {
                        var bones = smr.bones;
                        var boneIsEffective = new bool[bones.Length];
                        foreach (var bw in smr.sharedMesh.GetAllBoneWeights().Where(bw => bw.weight > 0f))
                        {
                            boneIsEffective[bw.boneIndex] = true;
                        }
                        for (var i = 0; i < bones.Length; i++)
                        {
                            var bone = bones[i];
                            if (boneIsEffective[i])
                            {
                                references.Add(bone);
                                // also add possibly end bone
                                if (bone.childCount == 1) references.Add(bone.GetChild(0));
                            }
                            else
                            {
                                bones[i] = smr.rootBone;
                            }
                        }
                        smr.bones = bones;
                    }

                    // register references
                    var serialized = new SerializedObject(component);
                    var it = serialized.GetIterator();
                    while (true)
                    {
                        Object obj = null;
                        switch (it.propertyType)
                        {
                            case SerializedPropertyType.ObjectReference:
                                obj = it.objectReferenceValue;
                                break;
                            case SerializedPropertyType.ExposedReference:
                                obj = it.exposedReferenceValue; 
                                break;
                        }

                        switch (it.name)
                        {
                            case "ignoreTransforms":
                                if (isVrcPhysBone) it.Next(false);
                                continue;
                        }

                        switch (obj)
                        {
                            case GameObject gameObjectRef:
                                references.Add(gameObjectRef.transform);
                                break;
                            case Component componentRef:
                                references.Add(componentRef.transform);
                                componentReferences.Add(componentRef);
                                break;
                        }

                        if (!it.Next(true)) break;
                    }
                }
            }

            var hierarchicalReferences = references
                .SelectMany(reference => reference.GetComponentsInParent<Transform>())
                .Distinct().ToArray();
            var gameObjectsToDestroy = modularObject.GetComponentsInChildren<Transform>()
                .Except(hierarchicalReferences)
                .Select(transform => transform.gameObject)
                .OrderByDescending(o => o.GetComponentsInParent<Transform>().Length)
                .ToArray();

            var componentsToDestroy = modularObject.GetComponentsInChildren<Component>()
                .Where(component => !(component is Transform))
                .Except(componentReferences)
                .ToArray();
                
            foreach (var component in componentsToDestroy)
            {
                Object.DestroyImmediate(component);
            }
            foreach (var gameObject in gameObjectsToDestroy)
            {
                Object.DestroyImmediate(gameObject);
            }

            return gameObjectsToDestroy.Length > 0;
        }

        protected abstract GameObject InstantiateModule(GameObject gameObject);
        protected abstract void IgnoreEmpty(Renderer renderer);
        protected abstract void IgnoreNonEmpty(Renderer renderer);

        static string BuildPrefabPath(string path, string moduleName) => Path.Combine(path, $"{moduleName}.prefab");
    }
}