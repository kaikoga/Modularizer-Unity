using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer.DataObjects
{
    [CustomPropertyDrawer(typeof(RendererSet))]
    public class RendererSetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IEnumerable<Renderer> Renderers()
            {
                foreach (SerializedProperty renderer in property.FindPropertyRelative(nameof(RendererSet.renderers)))
                {
                    yield return (Renderer)renderer.objectReferenceValue;
                }
            }

            var config = (ModularizerConfig)property.serializedObject.targetObject;
            var selectedRenderers = config.selectedRenderers = config.selectedRenderers ?? Array.Empty<Renderer>();

            var line = position;
            line.height = EditorGUIUtility.singleLineHeight;

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 1f;
            {
                var cell = line;
                cell.width = 30f;
                EditorGUI.PropertyField(cell, property.FindPropertyRelative(nameof(RendererSet.enabled)));
                line.xMin += 32f;
                cell = line;
                cell.width -= 108f;
                EditorGUI.PropertyField(cell, property.FindPropertyRelative(nameof(RendererSet.moduleName)));
                cell.x = cell.xMax + 4f;
                cell.width = 100f;
                EditorGUIUtility.labelWidth = 85f;
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    var isBaseModule = property.FindPropertyRelative(nameof(RendererSet.isBaseModule));
                    var isBaseModuleValue = EditorGUI.ToggleLeft(cell, "Base Module", isBaseModule.boolValue);
                    if (changed.changed) isBaseModule.boolValue = isBaseModuleValue;
                }

                EditorGUIUtility.labelWidth = 1f;
            }
            line.xMin += 32f;
            var containsAny = false;
            var containsAll = true;
            foreach (SerializedProperty renderer in property.FindPropertyRelative(nameof(RendererSet.renderers)))
            {
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var cell = line;
                cell.width = 30f;
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    var rendererObject = (Renderer)renderer.objectReferenceValue;
                    var contains = EditorGUI.Toggle(cell, selectedRenderers.Contains(rendererObject));
                    if (changed.changed)
                    {
                        if (contains)
                        {
                            ArrayUtility.Add(ref config.selectedRenderers, rendererObject);
                        }
                        else
                        {
                            ArrayUtility.Remove(ref config.selectedRenderers, rendererObject);
                        }
                    }

                    containsAny |= contains;
                    containsAll &= contains;
                }

                cell = line;
                cell.xMin += 32f;
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.PropertyField(cell, renderer);
                }
            }

            {
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var cell = line;
                cell.width = 30f;
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    var toggleAll = GUI.Toggle(cell, containsAll, "", !containsAny || containsAll ? "Toggle" : "ToggleMixed");
                    if (changed.changed)
                    {
                        if (toggleAll)
                        {
                            config.selectedRenderers = Renderers().ToArray();
                        }
                        else
                        {
                            config.selectedRenderers = Array.Empty<Renderer>();
                        }
                    }
                }

                if (containsAny)
                {
                    cell.x += 32f;
                    if (GUI.Button(cell, "+"))
                    {
                        property.FindPropertyRelative(nameof(RendererSet.command)).enumValueIndex = (int)RendererSet.Command.New;
                    }

                    cell.x += 32f;
                    if (GUI.Button(cell, "↑"))
                    {
                        property.FindPropertyRelative(nameof(RendererSet.command)).enumValueIndex = (int)RendererSet.Command.Up;
                    }

                    cell.x += 32f;
                    if (GUI.Button(cell, "↓"))
                    {
                        property.FindPropertyRelative(nameof(RendererSet.command)).enumValueIndex = (int)RendererSet.Command.Down;
                    }
                }
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var i = property.FindPropertyRelative(nameof(RendererSet.renderers)).arraySize + 1;
            return EditorGUIUtility.singleLineHeight * (i + 1) + EditorGUIUtility.standardVerticalSpacing * i;
        }
    }
}