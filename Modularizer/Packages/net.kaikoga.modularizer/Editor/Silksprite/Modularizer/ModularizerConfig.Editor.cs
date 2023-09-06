using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.Modularizer.DataObjects;
using Silksprite.Modularizer.Extensions;
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
        SerializedProperty _serializedModules;
        SerializedProperty _serializedUnpackPrefab;
        // SerializedProperty _serializedSetupMA;

        void OnEnable()
        {
            _config = (ModularizerConfig)target;

            _serializedExportDirectory = serializedObject.FindProperty(nameof(ModularizerConfig.exportDirectory));
            _serializedAvatarRoot = serializedObject.FindProperty(nameof(ModularizerConfig.avatarRoot));
            _serializedModules = serializedObject.FindProperty(nameof(ModularizerConfig.modules));
            _serializedUnpackPrefab = serializedObject.FindProperty(nameof(ModularizerConfig.unpackPrefab));
            // _serializedSetupMA = serializedObject.FindProperty(nameof(ModularizerConfig.setupMA));
        }

        void OnDisable()
        {
            _config.selectedRenderers = Array.Empty<Renderer>();
        }

        public override void OnInspectorGUI()
        {
            var avatarRoot = _serializedAvatarRoot.objectReferenceValue;
            EditorGUILayout.PropertyField(_serializedAvatarRoot);
            if (avatarRoot == null && _config.GetComponent<Animator>())
            {
                _serializedAvatarRoot.objectReferenceValue = _config.transform;
            }
            if (avatarRoot != _serializedAvatarRoot.objectReferenceValue && _serializedAvatarRoot.objectReferenceValue)
            {
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                CollectRenderers(_config, false);
            }

            EditorGUILayout.PropertyField(_serializedExportDirectory);
            if (GUILayout.Button("Select Folder"))
            {
                CreateFolder();
            }
            
            EditorGUILayout.PropertyField(_serializedUnpackPrefab);
            // EditorGUILayout.PropertyField(_serializedSetupMA);

            EditorGUILayout.Separator();

            if (_config.avatarRoot)
            {
                foreach (SerializedProperty module in _serializedModules)
                {
                    EditorGUILayout.PropertyField(module);
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (_config.avatarRoot && _config.modules.Any(module => module.command != RendererSet.Command.None))
            {
                ReorderRenderers(_config);
            }

            using (new EditorGUI.DisabledScope(!_config.avatarRoot))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Collect Renderers"))
                    {
                        CollectRenderers(_config, false);
                    }
                    if (GUILayout.Button("Reset Modules"))
                    {
                        CollectRenderers(_config, true);
                    }
                }

                EditorGUILayout.Separator();

                using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_config.exportDirectory)))
                {
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
            }
        }

        void CreateFolder()
        {
            ModularizerTools.SelectFolder("Select Export Directory" , ref _config.exportDirectory);
        }

        static void ReorderRenderers(ModularizerConfig config)
        {
            var allRenderers = config.AllRenderers;
            var newModules = new List<RendererSet>();
            for (var i = -1; i <= config.modules.Length; i++)
            {
                RendererSet TryGetModule(int index) => index >= 0 && index < config.modules.Length ? config.modules[index] : null;
                var module = TryGetModule(i); 
                var prev = TryGetModule(i - 1); 
                var next = TryGetModule(i + 1);

                var renderers = new List<Renderer>();
                if (module != null)
                {
                    renderers.AddRange(module.command == RendererSet.Command.None ? module.renderers : module.renderers.Where(renderer => !config.selectedRenderers.Contains(renderer)));
                }
                if (prev?.command == RendererSet.Command.Down)
                {
                    renderers.AddRange(prev.renderers.Where(config.selectedRenderers.Contains));
                }
                if (next?.command == RendererSet.Command.Up)
                {
                    renderers.AddRange(config.selectedRenderers.Where(next.renderers.Contains));
                }

                Renderer[] PrettifyRenderers(IEnumerable<Renderer> unsorted) => allRenderers.Where(unsorted.Contains).ToArray();
                if (module != null)
                {
                    newModules.Add(new RendererSet
                    {
                        enabled = module.enabled,
                        moduleName = module.moduleName,
                        renderers = PrettifyRenderers(renderers),
                        isBaseModule = module.isBaseModule,
                    });

                    if (module.command == RendererSet.Command.New)
                    {
                        var selections = config.selectedRenderers.Where(module.renderers.Contains).ToArray();
                        newModules.Add(new RendererSet
                        {
                            enabled = true,
                            moduleName = config.ModuleName(selections.FirstOrDefault()?.gameObject.name),
                            renderers = PrettifyRenderers(selections),
                            isBaseModule = false,
                        });
                    }
                }
                else
                {
                    newModules.Add(new RendererSet
                    {
                        enabled = true,
                        moduleName = config.ModuleName(renderers.FirstOrDefault()?.gameObject.name),
                        renderers = PrettifyRenderers(renderers),
                        isBaseModule = false,
                    });
                }
            }
            config.modules = newModules.Where(module => module.renderers.Any()).ToArray();
        }

        internal static void CollectRenderers(ModularizerConfig config, bool reset)
        {
            if (!config.avatarRoot) return;

            if (reset) config.modules = Array.Empty<RendererSet>();
            var allRenderers = config.AllRenderers.ToArray(); 
            var renderers = allRenderers
                .Where(renderer => !config.modules.Any(module => module.renderers.Contains(renderer)))
                .ToArray();
            // if (!renderers.Any()) return;
            config.modules = config.modules.Select(module => new RendererSet
                {
                    enabled = module.enabled,
                    moduleName = module.moduleName,
                    renderers = allRenderers.Where(renderer => module.renderers.Contains(renderer)).ToArray(),
                    isBaseModule = module.isBaseModule
                })
                .Concat(
                    renderers.GroupBy(renderer => renderer.sharedMaterial)
                        .Select((g, i) => (g, i))
                        .Select(gi => new RendererSet
                        {
                            enabled = true,
                            moduleName = config.ModuleName(gi.g.Key.name),
                            renderers = gi.g.ToArray(),
                            isBaseModule = gi.i + config.modules.Length == 0
                        }))
                .Where(module => module.renderers.Length > 0)
                .ToArray();
        }

        void Process(BaseProcessor processor)
        {
            var definition = ModularizeDefinition.Build(_config);
            if (definition == null) return;

            processor.Process(definition);
        }
    }
}