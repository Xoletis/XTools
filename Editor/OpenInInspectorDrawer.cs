using UnityEditor;
using UnityEngine;

namespace Xoletis.EditorTools
{
    [CustomPropertyDrawer(typeof(Object), true)]
    public class OpenInInspectorDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 20f;
        private const float Spacing = 2f;

        private static GUIContent _buttonContent;

        private static GUIContent ButtonContent =>
            _buttonContent ??= new GUIContent(CreateEyeIcon(), "Select this object so its Inspector shows up");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, position.height);
            var buttonRect = new Rect(fieldRect.xMax + Spacing, position.y, ButtonWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, label);

            using (new EditorGUI.DisabledScope(property.objectReferenceValue == null || property.hasMultipleDifferentValues))
            {
                if (GUI.Button(buttonRect, ButtonContent, EditorStyles.miniButton))
                {
                    Selection.activeObject = property.objectReferenceValue;
                    EditorGUIUtility.PingObject(property.objectReferenceValue);
                }
            }

            EditorGUI.EndProperty();
        }

        // Drawn procedurally (a ring + a pupil dot) instead of relying on a built-in
        // Unity icon name, since those aren't guaranteed to exist across versions.
        private static Texture2D CreateEyeIcon()
        {
            const int size = 16;
            var color = EditorGUIUtility.isProSkin
                ? new Color32(210, 210, 210, 255)
                : new Color32(50, 50, 50, 255);
            var transparent = new Color32(0, 0, 0, 0);

            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f - size / 2f) / (size / 2f);
                    float ny = (y + 0.5f - size / 2f) / (size / 2f);

                    float outer = nx * nx / (0.85f * 0.85f) + ny * ny / (0.45f * 0.45f);
                    float inner = nx * nx / (0.68f * 0.68f) + ny * ny / (0.28f * 0.28f);
                    float pupil = nx * nx / (0.16f * 0.16f) + ny * ny / (0.16f * 0.16f);

                    bool isRing = outer <= 1f && inner >= 1f;
                    bool isPupil = pupil <= 1f;

                    pixels[y * size + x] = isRing || isPupil ? color : transparent;
                }
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear
            };
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }
    }
}
