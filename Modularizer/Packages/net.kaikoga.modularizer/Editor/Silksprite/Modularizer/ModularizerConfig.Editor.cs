using System.Linq;
using Silksprite.Modularizer.DataObjects;
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
        ModularizerConfig _config;
        SerializedProperty _serializedExportDirectory;
        SerializedProperty _serializedAvatarRoot;
        SerializedProperty _serializedBodyRenderer;
        SerializedProperty _serializedRenderers;
        SerializedProperty _serializedUnpackPrefab;
        SerializedProperty _serializedSetupMA;

        void OnEnable()
        {
            _config = (ModularizerConfig)target;

            _serializedExportDirectory = serializedObject.FindProperty(nameof(ModularizerConfig.exportDirectory));
            _serializedAvatarRoot = serializedObject.FindProperty(nameof(ModularizerConfig.avatarRoot));
            _serializedBodyRenderer = serializedObject.FindProperty(nameof(ModularizerConfig.bodyRenderer));
            _serializedRenderers = serializedObject.FindProperty(nameof(ModularizerConfig.renderers));
            _serializedUnpackPrefab = serializedObject.FindProperty(nameof(ModularizerConfig.unpackPrefab));
            _serializedSetupMA = serializedObject.FindProperty(nameof(ModularizerConfig.setupMA));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_serializedExportDirectory);
            EditorGUILayout.PropertyField(_serializedAvatarRoot);
            EditorGUILayout.PropertyField(_serializedBodyRenderer);
            var iterator = _serializedRenderers.GetEnumerator();
            while (iterator.MoveNext())
            {
                EditorGUILayout.PropertyField((SerializedProperty)iterator.Current);
            }
            EditorGUILayout.PropertyField(_serializedUnpackPrefab);
            EditorGUILayout.PropertyField(_serializedSetupMA);
            serializedObject.ApplyModifiedProperties();
            
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
                if (_config.unpackPrefab)
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
            ModularizerTools.SelectFolder("Select Export Directory" , ref _config.exportDirectory);
        }

        void DetectBody()
        {
            if (!_config.avatarRoot) return;
            var renderers = _config.avatarRoot.GetComponentsInChildren<Renderer>(true);
            _config.renderers = renderers.Select(renderer => new RendererMapping(renderer, renderer.gameObject.name)).ToArray();
            _config.bodyRenderer = renderers.FirstOrDefault(renderer => renderer.enabled);
        }

        void Process(BaseProcessor processor)
        {
            var definition = ModularizeDefinition.Build(_config);
            if (definition == null) return;

            processor.Process(definition);
        }
    }
}