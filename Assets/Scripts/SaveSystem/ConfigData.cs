using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   class ConfigData
*   Container class for saving the settings data
*
*   Functions:
*   ConfigData(MenuManager menu), constructor
*/
[System.Serializable]
public class ConfigData
{
    public bool daltonicModeEnabled;
    public float musicVolume;
    public float sfxVolume;
    public bool isFullScreen;

    /*
    *   ConfigData(MenuManager menu)
    *
    *   Arguments: menu - MenuManager gameObject to get the data from
    *   
    *   Returns: void, constructor
    */
    public ConfigData(MenuManager menu)
    {
        daltonicModeEnabled = menu.enableDaltonicMode;
        musicVolume = menu.musicSlider.value;
        sfxVolume = menu.sfxSlider.value;
        isFullScreen = Screen.fullScreen;
    }
}
