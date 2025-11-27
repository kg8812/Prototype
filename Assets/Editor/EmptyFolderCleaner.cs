using System.IO;
using UnityEditor;
using UnityEngine;

public class EmptyFolderCleaner : Editor
{
    [MenuItem("AssetDataBase/Clean Empty Folder &F9")]
    public static void EmptyFolderClear()
    {
        Debug.Log("[EmptyFolderCleaner] Clean Start");
        var startDirection = Application.dataPath;
        CleanDirectory(startDirection);
        Debug.Log("[EmptyFolderCleaner] Clean End");
    }

    private static void CleanDirectory(string path)
    {
        var info = new DirectoryInfo(path);

        if (!info.Exists)
            return;

        var cInfo = info.GetDirectories("*", SearchOption.AllDirectories);

        foreach (var inf in cInfo)
        {
            if (HasFiles(inf.FullName))
                continue;

            Debug.Log($"[EmptyFolderCleaner] Clean {inf.FullName}");

            var di = new DirectoryInfo(inf.FullName);
            di.Delete(true);

            var file = new FileInfo(inf.FullName + ".meta");
            file.Delete();
        }
    }

    private static bool HasFiles(string dir)
    {
        var directories = Directory.GetDirectories(dir);
        var files = Directory.GetFiles(dir);
        return files.Length != 0;
    }
}