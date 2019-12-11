using System.IO;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Controller for saving (use JSON)
/// </summary>
public class SaveController
{
    public void Save(GameSession session)
    {
        using (StreamWriter writer = new StreamWriter(GameData.savePath))
        {
            string jsonStr = JsonUtility.ToJson(session, true);
            writer.Write(jsonStr);
        }
    }

    public void Load(out GameSession session)
    {
        if (!File.Exists(GameData.savePath))
        {
            session = null;
            return;
        }

        using (StreamReader reader = new StreamReader(GameData.savePath))
        {
            string jsonStr = reader.ReadToEnd();
            session = (GameSession) JsonUtility.FromJson(jsonStr, typeof(GameSession));
            if (session == null)
            {
                Debug.LogError("<color=red>Load : </color>cannot find save, load new game");
            }
        }
    }

    public void Clear()
    {
        if(File.Exists(GameData.savePath))
            File.Delete(GameData.savePath);
    }
}
