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
        private static GUIContent _autoFillButtonContent;
        private static GUIContent _addAndAssignButtonContent;

        private static GUIContent ButtonContent =>
            _buttonContent ??= new GUIContent(CreateEyeIcon(), "Select this object so its Inspector shows up");

        private static GUIContent AutoFillButtonContent =>
            _autoFillButtonContent ??= new GUIContent(CreateAutoFillIcon(),
                "Auto-assign the matching component found on this GameObject");

        private static GUIContent AddAndAssignButtonContent =>
            _addAndAssignButtonContent ??= new GUIContent(CreateAutoFillIcon(),
                "Add a new matching component to this GameObject and assign it");

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

            bool isEmpty = property.objectReferenceValue == null;
            GameObject autoFillTarget = null;
            System.Type autoFillType = null;
            var autoFillAction = AutoFillAction.None;
            if (isEmpty)
            {
                autoFillAction = GetAutoFillAction(property, out autoFillTarget, out autoFillType);
            }

            using (new EditorGUI.DisabledScope(autoFillAction == AutoFillAction.None && (isEmpty || property.hasMultipleDifferentValues)))
            {
                var content = autoFillAction switch
                {
                    AutoFillAction.AssignExisting => AutoFillButtonContent,
                    AutoFillAction.AddAndAssign => AddAndAssignButtonContent,
                    _ => ButtonContent
                };

                if (GUI.Button(buttonRect, content, EditorStyles.miniButton))
                {
                    switch (autoFillAction)
                    {
                        case AutoFillAction.AssignExisting:
                            property.objectReferenceValue = autoFillTarget.GetComponent(autoFillType);
                            break;
                        case AutoFillAction.AddAndAssign:
                            property.objectReferenceValue = Undo.AddComponent(autoFillTarget, autoFillType);
                            break;
                        default:
                            Selection.activeObject = property.objectReferenceValue;
                            EditorGUIUtility.PingObject(property.objectReferenceValue);
                            break;
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        private enum AutoFillAction
        {
            None,
            AssignExisting,
            AddAndAssign
        }

        // If the field is an empty Component (or interface) reference, offer to auto-fill it
        // from the GameObject currently being inspected: reuse a matching component if one is
        // already attached, otherwise add a new one (when the field type is a concrete
        // Component) so the user doesn't have to drag it in by hand.
        private AutoFillAction GetAutoFillAction(SerializedProperty property, out GameObject gameObject, out System.Type fieldType)
        {
            gameObject = null;
            fieldType = null;

            if (property.hasMultipleDifferentValues || fieldInfo == null)
            {
                return AutoFillAction.None;
            }

            gameObject = property.serializedObject.targetObject switch
            {
                Component component => component.gameObject,
                GameObject go => go,
                _ => null
            };

            if (gameObject == null)
            {
                return AutoFillAction.None;
            }

            fieldType = GetElementType(fieldInfo.FieldType);
            bool isComponentType = fieldType != null && typeof(Component).IsAssignableFrom(fieldType);
            bool isInterfaceType = fieldType is { IsInterface: true };

            if (!isComponentType && !isInterfaceType)
            {
                return AutoFillAction.None;
            }

            if (gameObject.GetComponent(fieldType) != null)
            {
                return AutoFillAction.AssignExisting;
            }

            return isComponentType && !fieldType.IsAbstract ? AutoFillAction.AddAndAssign : AutoFillAction.None;
        }

        private static System.Type GetElementType(System.Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType && typeof(System.Collections.IList).IsAssignableFrom(type))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
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

        // Drawn procedurally (an arrow dropping into a tray) to signal "auto-assign"
        // without depending on a built-in Unity icon name.
        private static Texture2D CreateAutoFillIcon()
        {
            const int size = 16;
            var color = EditorGUIUtility.isProSkin
                ? new Color32(140, 200, 255, 255)
                : new Color32(20, 90, 160, 255);
            var transparent = new Color32(0, 0, 0, 0);

            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isShaft = x is 7 or 8 && y is >= 2 and <= 8;

                    bool isHead = false;
                    if (y is >= 9 and <= 12)
                    {
                        int halfWidth = 4 - (y - 9);
                        isHead = Mathf.Abs(x - 7) <= halfWidth || Mathf.Abs(x - 8) <= halfWidth;
                    }

                    bool isTray = y == 13 && x is >= 3 and <= 12;

                    pixels[y * size + x] = isShaft || isHead || isTray ? color : transparent;
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
