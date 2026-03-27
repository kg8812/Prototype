#if !ES3GLOBAL_DISABLED
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ES3Internal
{
    [CustomEditor(typeof(ES3GlobalReferences))]
    [Serializable]
    public class ES3GlobalReferencesEditor : Editor
    {
        private ES3GlobalReferences _globalRefs;
        private bool isDraggingOver;
        private bool openReferences;

        private ES3GlobalReferences globalRefs
        {
            get
            {
                if (_globalRefs == null)
                    _globalRefs = (ES3GlobalReferences)serializedObject.targetObject;
                return _globalRefs;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This stores references to objects in Assets, allowing them to be referenced with the same ID between scenes.",
                MessageType.Info);

            if (EditorGUILayout.Foldout(openReferences, "References") != openReferences)
            {
                openReferences = !openReferences;
                if (openReferences)
                    openReferences = EditorUtility.DisplayDialog("Are you sure?",
                        "Opening this list will display every reference in the manager, which for larger projects can cause the Editor to freeze\n\nIt is strongly recommended that you save your project before continuing.",
                        "Open References", "Cancel");
            }

            // Make foldout drag-and-drop enabled for objects.
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                var evt = Event.current;

                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        isDraggingOver = true;
                        break;
                    case EventType.DragExited:
                        isDraggingOver = false;
                        break;
                }

                if (isDraggingOver)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        Undo.RecordObject(globalRefs, "Add References to Easy Save 3 Reference List");
                        foreach (var obj in DragAndDrop.objectReferences)
                            globalRefs.GetOrAdd(obj);
                        // Return now because otherwise we'll change the GUI during an event which doesn't allow it.
                        return;
                    }
                }
            }

            if (openReferences)
            {
                EditorGUI.indentLevel++;

                foreach (var kvp in globalRefs.refId)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(kvp.Key, typeof(Object), true);
                    EditorGUILayout.LongField(kvp.Value);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif