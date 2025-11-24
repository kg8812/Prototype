using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ActionTypeWindow : EditorWindow
{
    string typeName = "NewType";

    private void OnGUI()
    {
        minSize = new Vector2(300, 200);
        

        GUILayout.Label("Enter a Type name:");
        typeName = EditorGUILayout.TextField("Type Name", typeName);

        GUILayout.Space(20);

        string path = "Assets/BehaviourTree/Scripts/ActionNodes";


        if (GUILayout.Button("Create ActionNode Type"))
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                string tempType = typeName;
                string scriptName = typeName;

                if (typeName.Contains("ActionNode"))
                {
                    int idx = typeName.IndexOf("ActionNode", StringComparison.Ordinal);
                    tempType = typeName[..idx];
                }
                else
                {
                    scriptName = typeName + "ActionNode";
                }
                string folderPath = path + "/" + tempType;
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    
                    AssetDatabase.CreateFolder(path, tempType);
                }
                else
                {
                    Debug.LogError("이미 존재합니다");
                    return;
                }
                string templatePath = "Assets/BehaviourTree/Templates/ActionNodeTypeTemplate.txt";
                string scriptPath = folderPath + "/" + scriptName + ".cs";

                string template = File.ReadAllText(templatePath);
                template = template.Replace("#Name#", scriptName);
                
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
