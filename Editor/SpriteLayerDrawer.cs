using UnityEditor;
using UnityEngine;

namespace Xoletis.EditorTools
{
    [CustomPropertyDrawer(typeof(SpriteLayerAttribute))]
    public class SpriteLayerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        // EditorGUI.SortingLayerField exists but is internal, so the popup is built from
        // SortingLayer.layers (public runtime API) instead of relying on reflection.
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "[SpriteLayer] can only be used on an int field.", MessageType.Error);
                return;
            }

            var layers = SortingLayer.layers;
            var names = new string[layers.Length];
            int selectedIndex = 0;

            for (int i = 0; i < layers.Length; i++)
            {
                names[i] = layers[i].name;
                if (layers[i].id == property.intValue)
                {
                    selectedIndex = i;
                }
            }

            EditorGUI.BeginProperty(position, label, property);
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, names);
            property.intValue = layers[newIndex].id;
            EditorGUI.EndProperty();
        }
    }
}
