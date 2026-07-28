using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xoletis.EditorTools
{
    [InitializeOnLoad]
    public static class InspectorHistory
    {
        private const int MaxHistory = 50;

        private static readonly List<Object> History = new List<Object>();
        private static int _index = -1;
        private static bool _navigating;

        static InspectorHistory()
        {
            Selection.selectionChanged += OnSelectionChanged;
            Editor.finishedDefaultHeaderGUI += OnHeaderGUI;
        }

        private static void OnSelectionChanged()
        {
            if (_navigating)
            {
                _navigating = false;
                return;
            }

            var active = Selection.activeObject;
            if (active == null)
            {
                return;
            }

            if (_index >= 0 && _index < History.Count && History[_index] == active)
            {
                return;
            }

            if (_index < History.Count - 1)
            {
                History.RemoveRange(_index + 1, History.Count - _index - 1);
            }

            History.Add(active);
            _index = History.Count - 1;

            if (History.Count > MaxHistory)
            {
                int excess = History.Count - MaxHistory;
                History.RemoveRange(0, excess);
                _index -= excess;
            }
        }

        private static bool CanGoBack()
        {
            for (int i = _index - 1; i >= 0; i--)
            {
                if (History[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void GoBack()
        {
            int newIndex = _index - 1;
            while (newIndex >= 0 && History[newIndex] == null)
            {
                newIndex--;
            }

            if (newIndex < 0)
            {
                return;
            }

            _index = newIndex;
            _navigating = true;
            Selection.activeObject = History[_index];
        }

        private static GUIStyle _backButtonStyle;
        private static GUIContent _backButtonContent;

        private static GUIStyle BackButtonStyle
        {
            get
            {
                if (_backButtonStyle == null)
                {
                    // "IconButton" matches the flat, borderless look Unity uses for its
                    // own small icon-only controls; fall back to a plain button if the
                    // skin doesn't define it (e.g. after a Unity UI overhaul).
                    var native = GUI.skin.FindStyle("IconButton");
                    _backButtonStyle = new GUIStyle(native != null ? native : EditorStyles.miniButton)
                    {
                        fixedWidth = 0,
                        fixedHeight = 0
                    };
                }

                return _backButtonStyle;
            }
        }

        private static GUIContent BackButtonContent =>
            _backButtonContent ??= new GUIContent("◀", "Return to the previously inspected object");

        private static GUIStyle _rowStyle;

        private static GUIStyle RowStyle =>
            // Negative top margin pulls the row up against the header's last line
            // (e.g. Tag/Layer for a GameObject) instead of leaving Unity's default
            // gap between GUILayout blocks. Using style margin (rather than a
            // negative GUILayout.Space) keeps Layout/Repaint control IDs in sync,
            // so the button stays clickable.
            _rowStyle ??= new GUIStyle { margin = new RectOffset(0, 0, -4, 0) };

        private static void OnHeaderGUI(Editor editor)
        {
            if (editor.target != Selection.activeObject)
            {
                return;
            }

            using (new EditorGUILayout.HorizontalScope(RowStyle))
            {
                using (new EditorGUI.DisabledScope(!CanGoBack()))
                {
                    if (GUILayout.Button(BackButtonContent, BackButtonStyle, GUILayout.Width(18), GUILayout.Height(16)))
                    {
                        GoBack();
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
