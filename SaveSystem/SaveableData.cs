using System.IO;
using UnityEngine;

public class SaveableData
{
    static string Path { get => Application.persistentDataPath + "/";}

    // Write the GameSave data to a JSON file
    public void WriteToFile(string filename)
    {
        string jsonData = JsonUtility.ToJson(this);
        File.WriteAllText(Path + filename, jsonData);
    }

    // Read GameSave data from a JSON file
    public static T ReadFromFile<T>(string filename) where T : SaveableData
    {
        string path = Path + filename;

        if (!File.Exists(path))
        {
            //UnityEngine.Debug.LogError($"File not found: {path}");
            return null;
        }

        string jsonData = File.ReadAllText(path);
        T data = JsonUtility.FromJson<T>(jsonData);
        return data;
    }
}