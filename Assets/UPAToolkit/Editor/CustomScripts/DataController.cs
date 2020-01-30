using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataController : ScriptableObject
{
   
    public DataController()
    {

    }


    public static string SaveArmor(Armor armor)
    {

        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, armor);
        file.Close();

        return destination;

    }

    public static Armor LoadArmor(string destination)
    {

        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Armor data = (Armor)bf.Deserialize(file);
        file.Close();

        return data;
    }

}
