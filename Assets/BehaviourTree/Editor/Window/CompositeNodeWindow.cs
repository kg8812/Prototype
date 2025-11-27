using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CompositeNodeWindow : EditorWindow
{
    private string className = "";
    private string classOriginalName = "";
    private string scriptName = "NewScript";

    private void OnGUI()
    {
        minSize = new Vector2(300, 200);


        GUILayout.Label("Enter a script name:");
        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        GUILayout.Space(20);

        var path = "Assets/BehaviourTree/Scripts/CompositeNodes/" + className;


        if (GUILayout.Button("Create New Script"))
        {
            Debug.Log(className);
            if (!string.IsNullOrEmpty(scriptName))
            {
                var templatePath = "Assets/BehaviourTree/Templates/CompositeNodeTemplate.txt";
                var scriptPath = path + "/" + scriptName + ".cs";

                var template = File.ReadAllText(templatePath);
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

    public void Init(string className)
    {
        this.className = className;
        classOriginalName = className;
        if (className.Contains("CompositeNode"))
        {
            var idx = className.IndexOf("CompositeNode", StringComparison.Ordinal);
            this.className = className[..idx];
        }
    }
}