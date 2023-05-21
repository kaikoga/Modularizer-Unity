using System.Linq;
using Silksprite.Modularizer.Models;
using Silksprite.Modularizer.Processors;
using Silksprite.Modularizer.Tools;
using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer
{
    [CustomEditor(typeof(ModularizerConfig))]
    public class ModularizerConfigEditor : Editor
    {
        ModularizerConfig config;
        void OnEnable()
        {
            config = (ModularizerConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Select Folder"))
            {
                CreateFolder();
            }

            if (GUILayout.Button("Detect Body"))
            {
                DetectBody();
            }

            if (GUILayout.Button("Modularize"))
            {
                if (config.unpackPrefab)
                {
                    Process(new UnpackedPrefabProcessor());
                }
                else
                {
                    Process(new PrefabVariantProcessor());
                }
            }
        }

        void CreateFolder()
        {
            ModularizerTools.SelectFolder("Select Export Directory" , ref config.exportDirectory);
        }

        void DetectBody()
        {
            if (!config.avatarRoot) return;
            var renderers = config.avatarRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            config.renderers = renderers;
            config.bodyRenderer = renderers.FirstOrDefault(renderer => renderer.enabled);
        }

        void Process(BaseProcessor processor)
        {
            var definition = ModularizeDefinition.Build(config);
            if (definition == null) return;

            processor.Process(definition);
        }
    }
}