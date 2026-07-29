using System;
using UnityEditor;
using UnityEngine;

namespace Xoletis.EditorTools
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsVisible(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // Collapse the field entirely, including the spacing the default
            // layout would otherwise leave behind for it.
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVisible(property))
            {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        private bool IsVisible(SerializedProperty property)
        {
            var conditional = (ConditionalFieldAttribute)attribute;
            var condition = FindSiblingProperty(property, conditional.ConditionFieldName);

            if (condition == null)
            {
                Debug.LogWarning(
                    $"[ConditionalField] Field \"{conditional.ConditionFieldName}\" not found next to \"{property.propertyPath}\".");
                return true;
            }

            bool result = conditional.HasCompareValue
                ? MatchesCompareValue(condition, conditional.CompareValue)
                : IsTruthy(condition);

            return conditional.Inverse ? !result : result;
        }

        private static SerializedProperty FindSiblingProperty(SerializedProperty property, string fieldName)
        {
            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');

            string siblingPath = lastDot < 0
                ? fieldName
                : $"{path.Substring(0, lastDot)}.{fieldName}";

            return property.serializedObject.FindProperty(siblingPath);
        }

        private static bool IsTruthy(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex != 0;
                case SerializedPropertyType.Integer:
                    return property.intValue != 0;
                case SerializedPropertyType.Float:
                    return !Mathf.Approximately(property.floatValue, 0f);
                case SerializedPropertyType.String:
                    return !string.IsNullOrEmpty(property.stringValue);
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null;
                default:
                    return true;
            }
        }

        private static bool MatchesCompareValue(SerializedProperty property, object compareValue)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Enum:
                    // Compare by name rather than index: enumValueIndex is the position in
                    // declaration order, which does not always match the underlying enum value.
                    return property.enumNames[property.enumValueIndex] == compareValue.ToString();
                case SerializedPropertyType.Integer:
                    return property.intValue == Convert.ToInt32(compareValue);
                case SerializedPropertyType.Boolean:
                    return property.boolValue == Convert.ToBoolean(compareValue);
                case SerializedPropertyType.Float:
                    return Mathf.Approximately(property.floatValue, Convert.ToSingle(compareValue));
                case SerializedPropertyType.String:
                    return string.Equals(property.stringValue, compareValue as string, StringComparison.Ordinal);
                default:
                    return true;
            }
        }
    }
}
