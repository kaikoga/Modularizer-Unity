using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer
{
    public static class ModularizerMenuItems
    {
        [MenuItem("GameObject/Modularizer/Modularizer Config", false, 48)]
        static void CreateModularizer(MenuCommand _)
        {
            var avatarRoot = Selection.activeGameObject;
            if (avatarRoot && !avatarRoot.GetComponent<Animator>()) avatarRoot = null;

            var gameObject = new GameObject(avatarRoot ? $"Modularizer_{avatarRoot.name}" : "Modularizer");
            var modularizerConfig = gameObject.AddComponent<ModularizerConfig>();
            modularizerConfig.avatarRoot = avatarRoot ? avatarRoot.GetComponent<Transform>() : null;
        }
        
        [MenuItem("GameObject/Modularizer/Modularizer Config", true, 48)]
        static bool ValidateModularizer()
        {
            return Selection.activeGameObject;
        }
    }
}