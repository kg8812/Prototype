using System;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CompositeNodeWindow : EditorWindow
{
    string className = "";
    string classOriginalName = "";
    string scriptName = "NewScript";

    public void Init(string className)
    {
        this.className = className;
        classOriginalName = className;
        if (className.Contains("CompositeNode"))
        {
            int idx = className.IndexOf("CompositeNode", StringComparison.Ordinal);
            this.className = className[..idx];
        }
    }
    private void OnGUI()
    {
        minSize = new Vector2(300, 200);


        GUILayout.Label("Enter a script name:");
        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        GUILayout.Space(20);

        string path = "Assets/BehaviourTree/Scripts/CompositeNodes/" + className;


        if (GUILayout.Button("Create New Script"))
        {
            Debug.Log(className);
            if (!string.IsNullOrEmpty(scriptName))
            {
                string templatePath = "Assets/BehaviourTree/Templates/CompositeNodeTemplate.txt";
                string scriptPath = path + "/" + scriptName + ".cs";

                string template = File.ReadAllText(templatePath);
                template = template.Replace("#Name#", scriptName);
                template = template.Replace("#Name2#", classOriginalName);
                File.WriteAllText(scriptPath, template);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogWarning("Name is empty.");
            }

            Close();
        }
    }
}
