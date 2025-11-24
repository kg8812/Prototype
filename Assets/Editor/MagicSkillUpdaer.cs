using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Apis;

public class MagicSkillUpdater
{
    [MenuItem("Tools/Update MagicSkill IDs")]
    public static void UpdateMagicSkillIds()
    {
        string[] guids = AssetDatabase.FindAssets("t:MagicSkill");

        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MagicSkill skill = AssetDatabase.LoadAssetAtPath<MagicSkill>(path);

            if (skill == null)
                continue;

            // nameId 체크
            // if (skill.nameString != 0)
            // {
            //     int newSkillId = skill.nameString - 4000;
            //     if (skill.nameString != newSkillId)
            //     {
            //         skill.itemId = newSkillId;
            //         EditorUtility.SetDirty(skill);
            //         count++;
            //     }
            // }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Updated {count} MagicSkill assets.");
    }
}