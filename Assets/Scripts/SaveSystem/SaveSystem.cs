using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/*
*   class SaveSystem
*   Serializes the settings and game data in order to be saved or loaded
*
*   Functions:
*   SaveConfig(MenuManager _menu), void
*   LoadConfig(), ConfigData
*   SaveGame(GameManager manager), void
*   LoadGame(), GameData
*/
public static class SaveSystem
{
    /*
	*   SaveConfig(MenuManager _menu)
	*   Serializes the settings data
    *
    *   Arguments: _menu - MenuManager gameObject
	*
	*   Returns: void
	*/
    public static void SaveConfig(MenuManager _menu)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/config.master";

        FileStream stream = new FileStream(path, FileMode.Create);

        ConfigData cData = new ConfigData(_menu);

        formatter.Serialize(stream, cData);
        stream.Close();
    }

    /*
	*   SaveConfig(MenuManager _menu)
	*   Deserializes the settings data
	*
	*   Returns: void
	*/
    public static ConfigData LoadConfig()
    {
        string path = Application.persistentDataPath + "/config.master";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        ConfigData cData = formatter.Deserialize(stream) as ConfigData;
        stream.Close();

        return cData;
    }

    /*
	*   SaveGame(GameManager _menu)
	*   Serializes the game data
    *
    *   Arguments: manager - GameManager gameObject
	*
	*   Returns: void
	*/
    public static void SaveGame(GameManager manager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.master";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameData gData = new GameData(manager);

        formatter.Serialize(stream, gData);
        stream.Close();
    }

    /*
	*   LoadGame()
	*   Deserializes the settings data
	*
	*   Returns: void
	*/
    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/game.master";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        GameData gData = formatter.Deserialize(stream) as GameData;
        stream.Close();

        return gData;
    }
}
