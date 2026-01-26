using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static readonly string FILEPATH = Application.persistentDataPath + "/Save.json"; 

    public static void Save(GameSaveState save)
    {
        //Save as JSON
        string json = JsonUtility.ToJson(save);
        File.WriteAllText(FILEPATH, json); 
    }

    public static GameSaveState Load()
    {
        GameSaveState loadedSave = null;  
        if (File.Exists(FILEPATH))
        {
            string json = File.ReadAllText(FILEPATH);
            loadedSave = JsonUtility.FromJson<GameSaveState>(json); 
        }

        return loadedSave; 
    }

    //Returns true if there is a save file
    public static bool HasSave()
    {
        return File.Exists(FILEPATH); 
    }
}
