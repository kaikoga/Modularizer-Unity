using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer.DataObjects
{
    [CustomPropertyDrawer(typeof(RendererMapping))]
    public class RendererMappingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var left = position;
            left.xMax -= left.width / 2;
            var right = position;
            right.xMin += right.width / 2;

            var oldLabelWidth = EditorGUIUtility.labelWidth; 
            EditorGUIUtility.labelWidth = 1f;
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(left, property.FindPropertyRelative(nameof(RendererMapping.renderer)));
            }
            EditorGUI.PropertyField(right, property.FindPropertyRelative(nameof(RendererMapping.moduleName)));
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}