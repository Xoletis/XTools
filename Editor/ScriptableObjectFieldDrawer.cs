using UnityEditor;
using UnityEngine;

namespace Xoletis.EditorTools
{
    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class ScriptableObjectFieldDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 50f;
        private const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, position.height);
            var buttonRect = new Rect(fieldRect.xMax + Spacing, position.y, ButtonWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, label);

            using (new EditorGUI.DisabledScope(property.objectReferenceValue == null || property.hasMultipleDifferentValues))
            {
                if (GUI.Button(buttonRect, "Open"))
                {
                    Selection.activeObject = property.objectReferenceValue;
                    EditorGUIUtility.PingObject(property.objectReferenceValue);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
