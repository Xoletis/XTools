using System;
using UnityEditor;
using UnityEngine;

namespace Xoletis.EditorTools
{
    [CustomPropertyDrawer(typeof(EnumDictionary<,>), true)]
    public class EnumDictionaryDrawer : PropertyDrawer
    {
        private const string ValuesFieldName = "values";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
            {
                return height;
            }

            var valuesProperty = property.FindPropertyRelative(ValuesFieldName);
            for (int i = 0; i < valuesProperty.arraySize; i++)
            {
                height += EditorGUIUtility.standardVerticalSpacing +
                          EditorGUI.GetPropertyHeight(valuesProperty.GetArrayElementAtIndex(i));
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var names = Enum.GetNames(GetEnumType());
            var valuesProperty = property.FindPropertyRelative(ValuesFieldName);

            if (valuesProperty.arraySize != names.Length)
            {
                valuesProperty.arraySize = names.Length;
            }

            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;
            float y = foldoutRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            for (int i = 0; i < names.Length; i++)
            {
                var element = valuesProperty.GetArrayElementAtIndex(i);
                float height = EditorGUI.GetPropertyHeight(element);
                var rect = new Rect(position.x, y, position.width, height);
                EditorGUI.PropertyField(rect, element, new GUIContent(names[i]), true);
                y += height + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.indentLevel--;
        }

        private Type GetEnumType()
        {
            var type = fieldInfo.FieldType;

            while (type != null && (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(EnumDictionary<,>)))
            {
                type = type.BaseType;
            }

            if (type == null)
            {
                throw new InvalidOperationException("EnumDictionaryDrawer used on a field that is not an EnumDictionary<,>.");
            }

            return type.GetGenericArguments()[0];
        }
    }
}
