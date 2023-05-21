using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer.Processors
{
    public class PrefabVariantProcessor : BaseProcessor
    {
        protected override GameObject InstantiateModule(GameObject gameObject)
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

        protected override void IgnoreIsolated(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            var modularSkinnedMeshObject = skinnedMeshRenderer.gameObject;
            modularSkinnedMeshObject.SetActive(false);
            modularSkinnedMeshObject.tag = "EditorOnly";
        }

        protected override void IgnoreNonIsolated(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            var modularSkinnedMeshObject = skinnedMeshRenderer.gameObject;
            modularSkinnedMeshObject.SetActive(false);
            // modularSkinnedMeshObject.tag = "EditorOnly";
        }
    }
}