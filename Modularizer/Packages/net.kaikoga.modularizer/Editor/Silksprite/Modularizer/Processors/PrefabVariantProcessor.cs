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

        protected override void IgnoreEmpty(Renderer renderer)
        {
            var modularSkinnedMeshObject = renderer.gameObject;
            modularSkinnedMeshObject.SetActive(false);
            modularSkinnedMeshObject.tag = "EditorOnly";
        }

        protected override void IgnoreNonEmpty(Renderer renderer)
        {
            var modularSkinnedMeshObject = renderer.gameObject;
            modularSkinnedMeshObject.SetActive(false);
            // modularSkinnedMeshObject.tag = "EditorOnly";
        }
    }
}