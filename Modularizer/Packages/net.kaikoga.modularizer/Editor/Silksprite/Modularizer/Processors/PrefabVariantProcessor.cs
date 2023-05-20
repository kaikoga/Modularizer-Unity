using System.Linq;
using Silksprite.Modularizer.Models;
using Silksprite.Modularizer.Tools;
using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer.Processors
{
    public class PrefabVariantProcessor : BaseProcessor
    {
        public override void Process(ModularizeDefinition definition)
        {
            var rootObject = definition.RootObject;

            foreach (var module in definition.Modules)
            {
                var modularObject = InstantiatePrefab(rootObject);
                modularObject.name = module.ModuleName;
                foreach (var skinnedMeshRenderer in modularObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (module.Meshes.Contains(skinnedMeshRenderer.sharedMesh)) continue;
                    var modularSkinnedMeshObject = skinnedMeshRenderer.gameObject;
                    modularSkinnedMeshObject.SetActive(false);
                    modularSkinnedMeshObject.tag = "EditorOnly";
                }

                var assetPath = BuildPrefabPath(definition.ExportPath, module.ModuleName);
                ModularizerTools.EnsureDirectory(assetPath);
                PrefabUtility.SaveAsPrefabAsset(modularObject, assetPath);

                Object.DestroyImmediate(modularObject);
            }
        }

        static GameObject InstantiatePrefab(GameObject gameObject)
        {
            GameObject prefabRoot = null;
            if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            }
            else if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                prefabRoot = gameObject;
            }

            if (prefabRoot != null)
            {
                return PrefabUtility.InstantiatePrefab(prefabRoot) as GameObject;
            }
            else
            {
                return Object.Instantiate(gameObject);
            }
        }
    }
}