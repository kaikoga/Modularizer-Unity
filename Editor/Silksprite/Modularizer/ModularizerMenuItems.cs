using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer
{
    public static class ModularizerMenuItems
    {
        [MenuItem("GameObject/NuguminTool/NuguminTool Config", false, 48)]
        static void CreateModularizer(MenuCommand _)
        {
            var avatarRoot = Selection.activeGameObject;
            if (avatarRoot && !avatarRoot.GetComponent<Animator>()) avatarRoot = null;

            var gameObject = new GameObject(avatarRoot ? $"NuguminTool_{avatarRoot.name}" : "NuguminTool");
            var config = gameObject.AddComponent<ModularizerConfig>();

            if (avatarRoot)
            {
                config.avatarRoot = avatarRoot.GetComponent<Transform>();
                ModularizerConfigEditor.CollectRenderers(config, true);
            }
        }
    }
}