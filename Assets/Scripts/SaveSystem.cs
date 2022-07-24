using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    static readonly string SAVE_FILE = "landlubber.dat";
    private static GameData data;

    void Awake()
    {
        data = gameObject.GetComponent<GameData>();
    }

    public static void SaveGame()
    {
        string json = JsonUtility.ToJson(GameData.singleton);
        string filePath = GetFilePath(SAVE_FILE);

        if (File.Exists(filePath))
        {
            Debug.Log("Saving: overwriting existing save.");

            File.Delete(filePath);
            File.WriteAllText(filePath, json);
        }
        else
        {
            Debug.Log("Saving: Creating a new save.");

            File.WriteAllText(filePath, json);
            LoadGame();
        }
    }

    public static void LoadGame()
    {
        string filePath = GetFilePath(SAVE_FILE);

        if (File.Exists(filePath))
        {
            Debug.Log("Loading: Loading existing save.");
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, data);

            GameData.singleton = data;
        }
        else
        {
            Debug.Log("Loading: No save file found.");
            SaveGame();
        }
    }
    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
