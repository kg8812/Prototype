using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DecoratorTypeWindow : EditorWindow
{
    private string typeName = "NewType";

    private void OnGUI()
    {
        minSize = new Vector2(300, 200);


        GUILayout.Label("Enter a Type name:");
        typeName = EditorGUILayout.TextField("Type Name", typeName);

        GUILayout.Space(20);

        var path = "Assets/BehaviourTree/Scripts/DecoratorNodes";


        if (GUILayout.Button("Create DecoratorNode Type"))
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                var tempType = typeName;
                var scriptName = typeName;
                if (typeName.Contains("DecoratorNode"))
                {
                    var idx = typeName.IndexOf("DecoratorNode", StringComparison.Ordinal);
                    tempType = typeName[..idx];
                }
                else
                {
                    scriptName = typeName + "DecoratorNode";
                }

                var folderPath = path + "/" + tempType;

                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    AssetDatabase.CreateFolder(path, tempType);
                }
                else
                {
                    Debug.LogError("이미 존재합니다");
                    return;
                }

                var templatePath = "Assets/BehaviourTree/Templates/DecoratorNodeTypeTemplate.txt";
                var scriptPath = folderPath + "/" + scriptName + ".cs";

                var template = File.ReadAllText(templatePath);
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