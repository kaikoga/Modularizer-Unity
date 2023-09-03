using System;
using System.Collections.Generic;
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
            EditorGUILayout.PropertyField(_serializedAvatarRoot);

            EditorGUILayout.PropertyField(_serializedExportDirectory);
            if (GUILayout.Button("Select Folder"))
            {
                CreateFolder();
            }

            foreach (SerializedProperty module in _serializedModules)
            {
                EditorGUILayout.PropertyField(module);
            }
            EditorGUILayout.PropertyField(_serializedUnpackPrefab);
            // EditorGUILayout.PropertyField(_serializedSetupMA);
            serializedObject.ApplyModifiedProperties();

            var allRenderers = AllRenderers;
            if (_config.modules.Any(module => module.command != RendererSet.Command.None))
            {
                var newModules = new List<RendererSet>();
                for (var i = -1; i <= _config.modules.Length; i++)
                {
                    RendererSet TryGetModule(int index) => index >= 0 && index < _config.modules.Length ? _config.modules[index] : null;
                    var module = TryGetModule(i); 
                    var prev = TryGetModule(i - 1); 
                    var next = TryGetModule(i + 1);

                    var renderers = new List<Renderer>();
                    if (module != null)
                    {
                        renderers.AddRange(module.command == RendererSet.Command.None ? module.renderers : module.renderers.Where(renderer => !_config.selectedRenderers.Contains(renderer)));
                    }
                    if (prev?.command == RendererSet.Command.Down)
                    {
                        renderers.AddRange(prev.renderers.Where(_config.selectedRenderers.Contains));
                    }
                    if (next?.command == RendererSet.Command.Up)
                    {
                        renderers.AddRange(_config.selectedRenderers.Where(next.renderers.Contains));
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
                            var selections = _config.selectedRenderers.Where(module.renderers.Contains);
                            newModules.Add(new RendererSet
                            {
                                enabled = true,
                                moduleName = selections.FirstOrDefault()?.gameObject.name,
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
                            moduleName = renderers.FirstOrDefault()?.gameObject.name,
                            renderers = PrettifyRenderers(renderers),
                            isBaseModule = false,
                        });
                    }
                }
                _config.modules = newModules.Where(module => module.renderers.Any()).ToArray();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Collect Renderers"))
                {
                    CollectRenderers(false);
                }
                if (GUILayout.Button("Reset Modules"))
                {
                    CollectRenderers(true);
                }
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

        Renderer[] AllRenderers => _config.avatarRoot.GetComponentsInChildren<Renderer>(true);

        void CollectRenderers(bool reset)
        {
            if (!_config.avatarRoot) return;

            if (reset) _config.modules = Array.Empty<RendererSet>();
            var renderers = AllRenderers
                .Where(renderer => !_config.modules.Any(module => module.renderers.Contains(renderer)))
                .ToArray();
            if (!renderers.Any()) return;
            _config.modules = _config.modules.Select(module => new RendererSet
                {
                    enabled = module.enabled,
                    moduleName = module.moduleName,
                    renderers = module.renderers.Where(renderer => !renderers.Contains(renderer)).ToArray(),
                    isBaseModule = module.isBaseModule
                })
                .Concat(
                    renderers.GroupBy(renderer => renderer.sharedMaterial)
                        .Select((g, i) => (g, i))
                        .Select(gi => new RendererSet
                        {
                            enabled = true,
                            moduleName = gi.g.Key.name,
                            renderers = gi.g.ToArray(),
                            isBaseModule = gi.i + _config.modules.Length == 0
                        }))
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