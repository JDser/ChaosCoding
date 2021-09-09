using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoadHelper 
{
    private static string path = Application.persistentDataPath + "/progress.fun";

    public static void SaveData(Progress progress)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(path,FileMode.Create);

        formatter.Serialize(stream, progress);
        stream.Close();
    }

    public static Progress LoadData()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Progress data = formatter.Deserialize(stream) as Progress;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}