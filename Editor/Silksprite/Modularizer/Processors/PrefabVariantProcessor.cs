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
#if UNITY_2022_2_OR_NEWER
            Object.DestroyImmediate(renderer.gameObject);
#else
            var modularSkinnedMeshObject = renderer.gameObject;
            modularSkinnedMeshObject.SetActive(false);
            modularSkinnedMeshObject.tag = "EditorOnly";
#endif
        }

        protected override void IgnoreNonEmpty(Renderer renderer)
        {
#if UNITY_2022_2_OR_NEWER
            Object.DestroyImmediate(renderer);
#else
            // var modularSkinnedMeshObject = renderer.gameObject;
            renderer.enabled = false;
            // modularSkinnedMeshObject.tag = "EditorOnly";
#endif
        }
    }
}