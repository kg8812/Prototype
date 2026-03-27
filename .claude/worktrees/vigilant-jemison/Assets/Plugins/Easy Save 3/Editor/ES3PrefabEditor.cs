using System;
using ES3Internal;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ES3Prefab))]
[Serializable]
public class ES3PrefabEditor : Editor
{
    private bool openLocalRefs;
    private bool showAdvanced;

    public override void OnInspectorGUI()
    {
        var es3Prefab = (ES3Prefab)serializedObject.targetObject;
        EditorGUILayout.HelpBox(
            "Easy Save is enabled for this prefab, and can be saved and loaded with the ES3 methods.",
            MessageType.None);


        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Settings");
        if (showAdvanced)
        {
            EditorGUI.indentLevel++;
            es3Prefab.prefabId = EditorGUILayout.LongField("Prefab ID", es3Prefab.prefabId);
            EditorGUILayout.LabelField("Reference count", es3Prefab.localRefs.Count.ToString());
            EditorGUI.indentLevel--;

            openLocalRefs = EditorGUILayout.Foldout(openLocalRefs, "localRefs");
            if (openLocalRefs)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("It is not recommended to manually modify these.");

                foreach (var kvp in es3Prefab.localRefs)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(kvp.Key, typeof(Object), false);
                    EditorGUILayout.LongField(kvp.Value);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}