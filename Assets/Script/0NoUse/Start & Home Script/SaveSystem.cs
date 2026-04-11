using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER =
        Application.persistentDataPath + "/Saves/";

    private const string SAVE_FILE = "save.json";

    public static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(string json)
    {
        File.WriteAllText(SAVE_FOLDER + SAVE_FILE, json);
    }

    public static void DeleteSave()
    {
        string path = SAVE_FOLDER + SAVE_FILE;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }


    public static string Load()
    {
        string path = SAVE_FOLDER + SAVE_FILE;
        if (!File.Exists(path)) return null;

        return File.ReadAllText(path);
    }
}
