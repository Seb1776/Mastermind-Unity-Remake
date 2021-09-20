using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   class GameData
*   Container class for saving the game data
*
*   Functions:
*   GameData(GameManager menu), constructor
*/

[System.Serializable]
public class GameData
{
    public int winsForCodebreaker;
    public int winsForCodemaker;

    /*
    *   GameData(GameManager menu)
    *
    *   Arguments: menu - GameManager gameObject to get the data from
    *   
    *   Returns: void, constructor
    */
    public GameData(GameManager menu)
    {
        winsForCodebreaker = menu.winsCodebreaker;
        winsForCodemaker = menu.winsCodemaker;
    }
}
