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
            if (GUILayout.Button("Test"))
            {
                Process();
            }
        }

        void CreateFolder()
        {
            ModularizerTools.SelectFolder("Select Export Directory" , ref config.exportDirectory);
        }

        void Process()
        {
            var definition = ModularizeDefinition.Build(config);
            if (definition == null) return;

            new PrefabVariantProcessor().Process(definition);
        }
    }
}