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

        private static void OnHeaderGUI(Editor editor)
        {
            if (editor.target != Selection.activeObject)
            {
                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!CanGoBack()))
                {
                    if (GUILayout.Button(new GUIContent("◀ Back", "Return to the previously inspected object"),
                            EditorStyles.miniButton, GUILayout.Width(60)))
                    {
                        GoBack();
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
