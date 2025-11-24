using System;
using Apis;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using Apis.SkillTrees;
using Save.Schema;

public class AssetCreator
{
    // [MenuItem("AssetDataBase/CreateWeaponSkills")]
    // public static void CreateWeaponSkills()
    // {
    //     var types = typeof(MagicSkill).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(MagicSkill)));
    //     foreach (var type in types)
    //     {
    //         MagicSkill skill = ScriptableObject.CreateInstance(type) as MagicSkill;
    //         if (skill == null) continue;
    //         if (!Application.isPlaying)
    //         {
    //             string path;
    //
    //             switch (skill.Grade)
    //             {
    //                 case Weapon.WeaponGrade.Normal:
    //                     path = "Assets/06_ScriptableObjects/Skill/Weapon/Normal/";
    //                     break;
    //                 case Weapon.WeaponGrade.Rare:
    //                     path = "Assets/06_ScriptableObjects/Skill/Weapon/Rare/";
    //                     break;
    //                 case Weapon.WeaponGrade.Legend:
    //                     path = "Assets/06_ScriptableObjects/Skill/Weapon/Legend/";
    //                     break;
    //                 case Weapon.WeaponGrade.Noble:
    //                     path = "Assets/06_ScriptableObjects/Skill/Weapon/Noble/";
    //                     break;
    //                 case Weapon.WeaponGrade.Special:
    //                     path = "Assets/06_ScriptableObjects/Skill/Weapon/Special/";
    //                     break;
    //                 default:
    //                     continue;
    //             }
    //             if (!File.Exists(path + type.Name + ".asset"))
    //                 AssetDatabase.CreateAsset(skill, path + type.Name + ".asset");
    //         }
    //         AssetDatabase.SaveAssets();
    //     }
    // }
    
    [MenuItem("AssetDataBase/CreateSkillTrees")]
    public static void CreateSkillTree()
    {
        var types = typeof(SkillTree).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(SkillTree)));
        foreach (var type in types)
        {
            SkillTree tree = ScriptableObject.CreateInstance(type) as SkillTree;
            if (tree == null) continue;

            string typeName = type.Name;
            
            if (!Application.isPlaying)
            {
                string path;

                if (typeName.Contains("Ine"))
                {
                    path = "Assets/06_ScriptableObjects/Skill/SkillTree/Ine/";
                }
                else if (typeName.Contains("Jingburger"))
                {
                    path = "Assets/06_ScriptableObjects/Skill/SkillTree/Jingburger/";
                }
                else if (typeName.Contains("Lilpa"))
                {
                    path = "Assets/06_ScriptableObjects/Skill/SkillTree/Lilpa/";
                }
                else if (typeName.Contains("Jururu"))
                {
                    path = "Assets/06_ScriptableObjects/Skill/SkillTree/Jururu/";
                }
                else if (typeName.Contains("Gosegu"))
                {
                    path = "Assets/06_ScriptableObjects/Skill/SkillTree/Gosegu/";
                }
                else
                {
                    path = "Assets/06_ScriptableObjects/Skill/SkillTree/Viichan/";
                }

                if (!File.Exists(path + type.Name + ".asset"))
                    AssetDatabase.CreateAsset(tree, path + type.Name + ".asset");
            }
            AssetDatabase.SaveAssets();
        }
    }
}
