using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

/*
*   class MenuManager, inherits from MonoBehaviour
*   This is a general handler for the MainMenu
*
*   Functions():
*       Start(), void
*       SaveSettings(), void
*       LoadSettings(), void
*       PlayBackgroundMusic(), void
*       LeftArrowHTP(), void
*       RightArrowHTP(), void
*       ChangeBackgroundMusic(int), void
*       PlaySoundEffectButton(), void
*       SetFullScreen(), void
*       SetPlayerCodemaker(Dropdown), void
*       DaltonicMode(Toggle), void
*       SetResolution(int), void
*       SetBoardSize(Slider), void
*       SetVolumeMusic(float), void
*       SetVolumeSFX(float), void
*       SetGameSong(Dropdown), void
*       SetCodeBreakerName(InputField), void
*       SetCodeMakerName(InputField), void
*       SetBackground(), void
*       OpenMenu(), void
*       OpenMenuOnly(GameObject), void
*       CloseMenuOnly(GameObject), void
*       CloseMenu(GameObject), void
*       GoToScene(string), void
*       LoadASScene, IEnumerator
*       ExitGame(), void
*       ExitGameTrue(), IEnumerator
*/
public class MenuManager : MonoBehaviour
{
    public List<GameObject> backgroundPegs = new List<GameObject>();
    public List<PegInfo> availablePegs = new List<PegInfo>();
    public List<GameObject> menus = new List<GameObject>();
    public List<GameObject> htpPages = new List<GameObject>();
    public List<AudioClip> menuMusics = new List<AudioClip>();
    public AudioClip buttonSelectSFX;
    public AudioSource music;
    public AudioSource sfx;
    public Animator transitionToGame;
    public string codemakerMenuIs;
    public string codebreakerMenuName;
    public string codemakerMenuName;
    public string selectedSong;
    public int boardSizeMenu;
    public bool enableDaltonicMode;
    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;
    public int currentHTPIndex;
    [Header ("UI")]
    public Resolution[] resolutions;
    public Toggle dsToggle;
    public Text boardSizeText;
    public Text cbWins;
    public Text cmWins;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider boardSizeSlider;
    public Dropdown codemakerDD;
    public Dropdown songDD;
    public Dropdown resDD;
    public InputField codebreakerName;
    public InputField codemakerName;

    /*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
    void Start()
    {
        LoadSettings();
        resolutions = Screen.resolutions;
        resDD.ClearOptions();

        List<string> cResolutions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            cResolutions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
        }

        resDD.AddOptions(cResolutions);
        resDD.value = currentResolutionIndex;

        SetBackground();
        PlayBackgroundMusic();

        codemakerDD.onValueChanged.AddListener(delegate {
            SetPlayerCodemaker(codemakerDD);
        });

        codemakerName.onValueChanged.AddListener(delegate {
            SetCodemakerName(codemakerName);
        });
    
        codebreakerName.onValueChanged.AddListener(delegate {
            SetCodebreakerName(codebreakerName);
        });

        songDD.onValueChanged.AddListener(delegate {
            SetGameSong(songDD);
        });

        boardSizeSlider.onValueChanged.AddListener(delegate {
            SetBoardSize(boardSizeSlider);
        });

        dsToggle.onValueChanged.AddListener(delegate {
            DaltonicMode(dsToggle);
        });
    }

    /*
	*   SaveSettings()
	*   Save the config file to be used later
	*
	*   Returns: void
	*/
    public void SaveSettings()
    {
        SaveSystem.SaveConfig(this);
    }

    /*
	*   LoadSettings()
	*   Loads the config and applies it
	*
	*   Returns: void
	*/
    public void LoadSettings()
    {
        string path = Application.persistentDataPath + "/config.master";
        string pathG = Application.persistentDataPath + "/game.master";

        if (File.Exists(path))
        {
            ConfigData cData = SaveSystem.LoadConfig();

            sfxSlider.value = cData.sfxVolume;
            musicSlider.value = cData.musicVolume;
            SetVolumeMusic(cData.musicVolume);
            SetVolumeSFX(cData.sfxVolume);
            enableDaltonicMode = dsToggle.isOn = cData.daltonicModeEnabled;
            SetFullScreen(cData.daltonicModeEnabled);
            Screen.fullScreen = cData.isFullScreen;
        }

        else
            SaveSettings();
        
        if (File.Exists(pathG))
        {
            GameData gData = SaveSystem.LoadGame();

            cbWins.text = gData.winsForCodebreaker.ToString();
            cmWins.text = gData.winsForCodemaker.ToString();
        }
    }

    /*
	*   PlayBackgroundMusic()
	*   Plays a random song at the start
	*
	*   Returns: void
	*/
    void PlayBackgroundMusic()
    {
        int randomIndex = Random.Range(0, menuMusics.Count);
        music.loop = true;
        music.clip = menuMusics[randomIndex];
        music.Play();
    }

    /*
	*   LeftArrowHTP()
	*   Shows the How To Play backwards
	*
	*   Returns: void
	*/
    public void LeftArrowHTP()
    {
        currentHTPIndex--;

        if (currentHTPIndex < 0)
            currentHTPIndex = htpPages.Count - 1;
        
        for (int i = 0; i < htpPages.Count; i++)
        {
            if (htpPages[i] != htpPages[currentHTPIndex])
                htpPages[i].SetActive(false);
        }

        htpPages[currentHTPIndex].SetActive(true);
    }

    /*
	*   RightArrowHTP()
	*   Shows the How To Play forwards
	*
	*   Returns: void
	*/
    public void RightArrowHTP()
    {
        currentHTPIndex++;

        if (currentHTPIndex >= htpPages.Count)
            currentHTPIndex = 0;
        
        for (int i = 0; i < htpPages.Count; i++)
        {
            if (htpPages[i] != htpPages[currentHTPIndex])
                htpPages[i].SetActive(false);
        }

        htpPages[currentHTPIndex].SetActive(true);
    }

    /*
	*   ChangeBackgroundMusic()
	*   Changes the background song
    *
    *   Arguments: audioClipIndex - Index of the song 
	*
	*   Returns: void
	*/
    public void ChangeBackgroundMusic(int audioClipIndex)
    {   
        if (music.clip != menuMusics[audioClipIndex])
        {
            music.loop = true;
            music.clip = menuMusics[audioClipIndex];
            music.Play();
        }
    }

    /*
	*   PlaySoundEffectButton()
	*   Just plays a simple 'pop!' sound effect for buttons 
	*
	*   Returns: void
	*/
    public void PlaySoundEffectButton()
    {
        sfx.PlayOneShot(buttonSelectSFX);
    }

    /*
	*   SetFullScreen()
	*   Toggles between fullscreen and windowed
    *
    *   Arguments: isFullScreen - Toggle's boolean
	*
	*   Returns: void
	*/
    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    /*
	*   SetPlayerCodemaker()
	*   Sets the type of Codemaker, Player or AI
    *
    *   Arguments: cmDown - Codemaker dropdown
	*
	*   Returns: void
	*/
    public void SetPlayerCodemaker(Dropdown cmDown)
    {
        if (cmDown.value == 1)
        {
            codemakerMenuIs = "AI";
            codemakerName.text = "";
            codemakerName.gameObject.SetActive(false);
        }

        else
        {
            codemakerMenuIs = "Player";
            codemakerName.gameObject.SetActive(true);
        }
    }

    /*
	*   DaltonicMode()
	*   Toggles the daltonic support mode
    *
    *   Arguments - DaltonicMode Toggle
	*
	*   Returns: void
	*/
    public void DaltonicMode(Toggle dsT)
    {
        enableDaltonicMode = dsT.isOn;
    }

    /*
	*   SetResolution()
	*   Changes the game resolution
    *
    *   Arguments: resIndex - Resolution index
	*
	*   Returns: void
	*/
    public void SetResolution (int resIndex)
    {
        Resolution res = resolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    /*
	*   SetBoardSize()
	*   Changes the board size
	*
    *   Arguments: bsS - Board size slider
    *
	*   Returns: void
	*/
    public void SetBoardSize(Slider bsS)
    {
        boardSizeMenu = (int)bsS.value;
        boardSizeText.text = bsS.value.ToString();
    }

    /*
	*   SetVolumeMusic()
	*   Changes the volume of music mixer
	*
    *   Arguments: musVal - Volume of the mixer
    *
	*   Returns: void
	*/
    public void SetVolumeMusic(float musVal)
    {
        musicMixer.SetFloat("MusicVolume", musVal);
    }

    /*
	*   SetVolumeSFX()
	*   Changes the volume of sfx mixer
	*
    *   Arguments: sfxVal - Volume of the mixer
    *
	*   Returns: void
	*/
    public void SetVolumeSFX(float sfxVal)
    {
        sfxMixer.SetFloat("SFXVolume", sfxVal);
    }

    /*
	*   SetGameSong()
	*   Sets the game song
	*
    *   Arguments: _song, Dropdown containing the songs
    *
	*   Returns: void
	*/
    public void SetGameSong(Dropdown _song)
    {
        switch (_song.value)
        {
            case 0:
                selectedSong = "gunmetalgrey";
            break;
        
            case 1:
                selectedSong = "homeinvasion";
            break;
        
            case 2:
                selectedSong = "iwillgiveyoumyall";
            break;
            
            case 3:
                selectedSong = "codesilver";
            break;

            case 4:
                selectedSong = "pimpedoutgetaway";
            break;

            case 5:
                selectedSong = "hohoho";
            break;

            case 6:
                selectedSong = "razormind";
            break;

            case 7:
                selectedSong = "8bitsarescary";
            break;
        }
    }

    /*
	*   SetCodemakerName()
	*   An input field that sets the codemaker's name
	*
    *   Arguments: cmF - The input field in question
    *
	*   Returns: void
	*/
    public void SetCodemakerName(InputField cmF)
    {
        codemakerMenuName = cmF.text;
    }

    /*
	*   SetCodebreakerName()
	*   An input field that sets the codebreaker's name
	*
    *   Arguments: cbF - The input field in question
    *
	*   Returns: void
	*/
    public void SetCodebreakerName(InputField cbF)
    {
        codebreakerMenuName = cbF.text;
    }

    /*
	*   SetBackground()
	*   Sets the pattern for the menu background
    *
	*   Returns: void
	*/
    void SetBackground()
    {
        for (int i = 0; i < backgroundPegs.Count; i++)
        {
            int randomColor = Random.Range(0, availablePegs.Count);

            backgroundPegs[i].GetComponent<SpriteRenderer>().color = availablePegs[randomColor].pegColor;
            backgroundPegs[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = availablePegs[randomColor].pegColor;
            backgroundPegs[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = availablePegs[randomColor].daltonicIconSprite;
        }
    }

    /*
	*   OpenMenu()
	*   Opens the menu in the args and close the others
	*
    *   Arguments: menuToOpen - Menu to open
    *
	*   Returns: void
	*/
    public void OpenMenu(GameObject menuToOpen)
    {
        for (int i = 0; i < menus.Count; i++)
            menus[i].SetActive(false);
        
        menuToOpen.SetActive(true);
    }

    /*
	*   OpenMenuOnly()
	*   Opens the menu in the args
	*
    *   Arguments: menuToOpen - Menu to open
    *
	*   Returns: void
	*/
    public void OpenMenuOnly(GameObject menuToOpen)
    {
        menuToOpen.SetActive(true);
    }

    /*
	*   CloseMenuOnly()
	*   Closes the menu in the args without the others
	*
    *   Arguments: menuToOpen - Menu to close
    *
	*   Returns: void
	*/
    public void CloseMenuOnly(GameObject menuToOpen)
    {
        menuToOpen.SetActive(false);
    }

    /*
	*   CloseMenu()
	*   Closes the menu in the args
	*
    *   Arguments: menuToOpen - Menu to close
    *
	*   Returns: void
	*/
    public void CloseMenu(GameObject menuToClose)
    {
        for (int i = 0; i < menus.Count; i++)
            menus[i].SetActive(false);
    }

    /*
	*   GoToScene()
	*   Determines whether or not the game is ready to start
	*
    *   Arguments: sceneName - Scene to load
    *
	*   Returns: void
	*/
    public void GoToScene(string sceneName)
    {
        if (codebreakerName.text != "")
        {
            if (codemakerDD.value == 1 && codemakerName.text == "")
            {
                music.Stop();
                transitionToGame.SetBool("go", true);
                transform.parent = null;
                DontDestroyOnLoad(this.gameObject);
                StartCoroutine(LoadASScene(sceneName));
            }

            else if (codemakerDD.value == 0 && codemakerName.text != "")
            {
                music.Stop();
                transitionToGame.SetBool("go", true);
                transform.parent = null;
                DontDestroyOnLoad(this.gameObject);
                StartCoroutine(LoadASScene(sceneName)); 
            }
        }
    }

    /*
	*   LoadASScene()
	*   Loads a scene asyncronously
	*
    *   Arguments: _sceneName - Scene to load
	*/
    IEnumerator LoadASScene(string _sceneName)
    {
        yield return new WaitForSeconds(1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(_sceneName);

        while (!op.isDone)
        {
            yield return null;
        }
    }
    
    /*
	*   ExitGame()
	*   Prepares the game for exiting
    *
	*   Returns: void
	*/
    public void ExitGame()
    {
        music.Stop();
        transitionToGame.SetBool("go", true);
        StartCoroutine(ExitGameTrue());
    }

    /*
	*   ExitGameTrue()
	*   Closes the game after 1.5 seconds
    *
	*   Returns: void
	*/
    IEnumerator ExitGameTrue()
    {
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }
}
