using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

/*
*   class GameManager, Inherits from MonoBehaviour
*   This is a general handler for all other functions and GameObjects
*
*   Functions:
*       Start(), void
*       Update(), void
*       SetPegOnBoard(), void
*		SaveGameProgress(), void
*		LoadGameProgress(), void
*		ChangeScene(string), void
*		LoadASScene(string), IEnumerator
*		TriggerConfetti(), void
*		SetCluePegAfterTime(string, CluePeg), void
*		SetReferencePegs(), void
*		RemoveStartPanel(), void
*		SetPegLimit(), void
*		ShowFinalPeg(), void
*		GenerateMasterCollection(), void
*       DetectPegOnBoard(), void
*       CheckCurrentRow(), void
*       SelectAPeg(), void
*       DeSelectAPeg(), void
*		CheckTurnRows(), void
*		DetectCluePegOnBoard(), void
*		SetCluePegOnBoard(), void
*/
public class GameManager : MonoBehaviour
{
	public enum Turn {Codemaker, Codebreaker, Empty}
	public Turn currentTurn;
	public enum CodebreakerIs {Player, AI}
	public CodebreakerIs currentCodebreaker;
	public enum CodemakerIs {Player, AI}
	public CodemakerIs currentCodemaker;
	public enum CodemakerAIClues {Normal, Swap}
	public CodemakerAIClues codeMakerClueMode;
	public string codebreakerName;
	public string codemakerName;
	public Text winText;
	public bool codeMakerRepeatNumber;
	public float timeBtwClues;
	public int winsCodemaker;
	public int winsCodebreaker;
	[Range(4, 6)]
	public int boardSize;
	public bool daltonicMode;
	public Animator codebreakerAnimator;
	public Animator codemakerAnimator;
	public Animator startPanel;
	public Animator startPanelVSAI;
	public Animator finalPegs;
	public Animator winTextAnimator;
	public Animator cameraWinAnimation;
	public Animator turnsWarning;
	public Animator transitionStart;
	public Animator readyBreakerButton, readyMakerButton;
	public Text turnsWarningText;
	public AudioClip selectPegSFX, placePegSFX, removePegSFX, invalidRowSFX;
	public GameObject goToMenuButton;
	public GameObject confettiWin;
	public GameObject makerPlayerBreakerPlayerBanner;
	public GameObject makerAIBreakerPlayerBanner;
	public GameObject makerPlayerBreakerAIBanner;
	public List<SelectorPeg> selectorPegs = new List<SelectorPeg>();
	public SelectorPeg selectedPegTo;
	public SelectorPeg previousSelectedPeg;
	public Peg currentHoverPeg;
	public CluePeg currentHoverCluePeg;
	public PegCollection masterCollection;
	public List<float> yValuesForRows = new List<float>();
	public List<PegCollection> rows = new List<PegCollection>();
	public List<SetPegStart> codemakerChoose = new List<SetPegStart>();
	public List<SetPegStart> codemakerChooseVSAI = new List<SetPegStart>();
	public List<PegInfo> allInfo = new List<PegInfo>();
	public List<string> cpuNames = new List<string>();
	public MenuManager menu;
	public bool canPlay;
	[Range(0, 10)]
	public int usingCurrentRow;

	Transform ogCodebreakerPegs, ogCodemakerPegs;
	MusicSystem music;
	
	/*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
	void Start()
	{
		LoadGameProgress();
		menu = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
		music = GameObject.FindGameObjectWithTag("MusicSystem").GetComponent<MusicSystem>();

		if (menu != null)
		{
			music.GetSong(menu.selectedSong);
			codebreakerName = menu.codebreakerMenuName;
			codemakerName = menu.codemakerMenuName;
			boardSize = menu.boardSizeMenu;
			daltonicMode = menu.enableDaltonicMode;

			if (menu.codemakerMenuIs == "Player")
				currentCodemaker = CodemakerIs.Player;
			
			else if (menu.codemakerMenuIs == "AI")
			{
				currentCodemaker = CodemakerIs.AI;

				int randNameIndex = Random.Range(0, cpuNames.Count);
				codemakerName = cpuNames[randNameIndex];
			}
			
			Destroy(menu.gameObject);
		}

		if (currentCodemaker == CodemakerIs.AI)
		{
			if (Random.value >= .5f)
				codeMakerClueMode = CodemakerAIClues.Swap;
			
			if (Random.value >= .5f)
				codeMakerRepeatNumber = true;
		}

		if (boardSize != 6)
			SetPegLimit();
		
		currentTurn = Turn.Codebreaker;
		readyBreakerButton.SetBool("show", true);

		if (currentCodebreaker == CodebreakerIs.Player && currentCodemaker == CodemakerIs.Player)
		{
			makerAIBreakerPlayerBanner.SetActive(false);
			makerPlayerBreakerPlayerBanner.SetActive(true);
			makerPlayerBreakerAIBanner.SetActive(false);
			readyBreakerButton.SetBool("show", true);
			readyMakerButton.SetBool("show", false);
		}
		
		else if (currentCodebreaker == CodebreakerIs.Player && currentCodemaker == CodemakerIs.AI)
		{
			makerAIBreakerPlayerBanner.SetActive(true);
			makerPlayerBreakerPlayerBanner.SetActive(false);
			makerPlayerBreakerAIBanner.SetActive(false);

			readyBreakerButton.SetBool("show", true);
			readyMakerButton.SetBool("show", false);
			GenerateMasterCollection();
		}

		else if (currentCodebreaker == CodebreakerIs.AI && currentCodemaker == CodemakerIs.Player)
		{
			makerAIBreakerPlayerBanner.SetActive(false);
			makerPlayerBreakerPlayerBanner.SetActive(false);
			makerPlayerBreakerAIBanner.SetActive(true);
		}

		else if (currentCodebreaker == CodebreakerIs.AI && currentCodemaker == CodemakerIs.AI)
			Debug.LogError("wtf bro");

		CheckTurnRows();
	}

	/*
	*   Update()
	*   Gets executed every frame
	*
	*   Returns: void
	*/
	void Update()
	{	
		if (canPlay)
		{
			if (currentTurn == Turn.Codemaker)
				finalPegs.transform.GetChild(0).GetComponent<Button>().enabled = true;
			else
				finalPegs.transform.GetChild(0).GetComponent<Button>().enabled = false;

			if (Input.GetKeyDown(KeyCode.O))
				CheckCurrentRow();

			if (currentHoverPeg != null)
				SetPegOnBoard();
			
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				ChangeScene("Menu");
				canPlay = false;
			}
			
			if (currentHoverCluePeg != null)
				SetCluePegOnBoard();

			if (selectedPegTo != null && !selectedPegTo.isCluePeg)
				DetectPegOnBoard();
			
			if (selectedPegTo != null && selectedPegTo.isCluePeg)
				DetectCluePegOnBoard();
			
			if (selectedPegTo == null)
			{
				if (Input.GetMouseButtonDown(1))
				{
					RaycastHit2D pegged = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

					if (pegged.collider != null)
					{
						if (pegged.collider.CompareTag("Peg"))
						{
							if (pegged.collider.GetComponent<Peg>() != null && !pegged.collider.GetComponent<Peg>().ready && currentTurn == Turn.Codebreaker)
							{
								music.PlaySoundEffect(removePegSFX);
								pegged.collider.GetComponent<Peg>().DeSetPegData();
							}
						}

						if (pegged.collider.CompareTag("CluePeg"))
						{
							if (pegged.collider.GetComponent<CluePeg>() != null && currentTurn == Turn.Codemaker)
							{
								music.PlaySoundEffect(removePegSFX);
								pegged.collider.GetComponent<CluePeg>().DeSetCluePeg();
							}
						}
					}
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (selectedPegTo == null || selectedPegTo != previousSelectedPeg)
					SelectAPeg();
			}

			if (Input.GetMouseButtonDown(1))
				if (selectedPegTo != null)
					DeSelectAPeg();
			
			if (currentTurn == Turn.Codemaker)
			{
				codebreakerAnimator.SetBool("show", false);
				codemakerAnimator.SetBool("show", true);

				foreach (SelectorPeg sp in selectorPegs)
					if (sp.isCluePeg)
						sp.canBeSelected = true;
					
				foreach (SelectorPeg sp in selectorPegs)
					if (!sp.isCluePeg)
						sp.canBeSelected = false;
			}

			else if (currentTurn == Turn.Codebreaker)
			{
				codebreakerAnimator.SetBool("show", true);
				codemakerAnimator.SetBool("show", false);
			
				foreach (SelectorPeg sp in selectorPegs)
					if (sp.isCluePeg)
						sp.canBeSelected = false;
				
				foreach (SelectorPeg sp in selectorPegs)
					if (!sp.isCluePeg)
						sp.canBeSelected = true;
			}

			else if (currentTurn == Turn.Empty)
			{
				codebreakerAnimator.SetBool("show", false);
				codemakerAnimator.SetBool("show", false);

				foreach (SelectorPeg sp in selectorPegs)
					if (sp.isCluePeg)
						sp.canBeSelected = false;
				
				foreach (SelectorPeg sp in selectorPegs)
					if (!sp.isCluePeg)
						sp.canBeSelected = false;
			}
		}

		else
		{
			currentTurn = Turn.Empty;

			if (currentTurn == Turn.Empty)
			{
				codebreakerAnimator.SetBool("show", false);
				codemakerAnimator.SetBool("show", false);

				foreach (SelectorPeg sp in selectorPegs)
					if (sp.isCluePeg)
						sp.canBeSelected = false;
				
				foreach (SelectorPeg sp in selectorPegs)
					if (!sp.isCluePeg)
						sp.canBeSelected = false;
			}
		}
	}

	/*
	*   SetPegOnBoard()
	*   From an already selected PegSelector sets an empty peg into a peg type.
	*
	*   Returns: void
	*/
	void SetPegOnBoard()
	{
		if (Input.GetMouseButtonDown(0))
		{
			currentHoverPeg.isEmpty = false;
			currentHoverPeg.sr.sprite = currentHoverPeg.pegSprite;
			currentHoverPeg.daltIconSr.color = selectedPegTo.pi.pegColor;
			currentHoverPeg.sr.color = selectedPegTo.pi.pegColor;
			currentHoverPeg.isHovering = false;
			currentHoverPeg.setPI = selectedPegTo.pi;
			currentHoverPeg.pegCodeValue = selectedPegTo.pi.codeValue;
			selectedPegTo.regenerate = true;
			music.PlaySoundEffect(placePegSFX);
			DeSelectAPeg();
			currentHoverPeg = null;
		}
	}

	/*
	*	SaveGameProgress()
	*	Saves the game progress
	*
	*	Returns: void
	*/
	public void SaveGameProgress()
	{
		SaveSystem.SaveGame(this);
	}

	public void LoadGameProgress()
	{
		string path = Application.persistentDataPath + "/game.master";

		if (File.Exists(path))
        {
            GameData gData = SaveSystem.LoadGame();

            winsCodebreaker = gData.winsForCodebreaker;
			winsCodemaker = gData.winsForCodemaker;
        }

        else
            SaveGameProgress();
	}

	/*
	*   SetCluePegOnBoard()
	*   From an already selected PegSelector sets an empty Clue Peg into a Clue Peg type.
	*
	*   Returns: void
	*/
	void SetCluePegOnBoard()
	{
		if (Input.GetMouseButtonDown(0))
		{
			currentHoverCluePeg.isEmpty = false;
			currentHoverCluePeg.sr.sprite = currentHoverCluePeg.pegSprite;

			switch(selectedPegTo.currentClue)
			{	
				case SelectorPeg.ClueState.RightColor_RightPosition:
					currentHoverCluePeg.sr.color = selectedPegTo.redCluePeg;
					currentHoverCluePeg.currentClue = CluePeg.ClueState.RightColor_RightPosition;
				break;

				case SelectorPeg.ClueState.RightColor_WrongPosition:
					currentHoverCluePeg.sr.color = selectedPegTo.whiteCluePeg;
					currentHoverCluePeg.currentClue = CluePeg.ClueState.RightColor_WrongPosition;
				break;
			}

			currentHoverCluePeg.isHovering = false;
			selectedPegTo.regenerate = true;
			music.PlaySoundEffect(placePegSFX);
			DeSelectAPeg();
			currentHoverCluePeg = null;
		}
	}

	/*
	*   DetectPegOnBoard()
	*   From an already selected PegSelector sets an empty peg into a peg type.
	*
	*   Returns: void
	*/
	void DetectPegOnBoard()
	{
		RaycastHit2D pegged = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (pegged.collider != null)
		{
			if (pegged.collider.CompareTag("Peg"))
			{
				if (pegged.collider.GetComponent<Peg>() != null && !pegged.collider.GetComponent<Peg>().ready)
				{
					currentHoverPeg = pegged.collider.GetComponent<Peg>();
					currentHoverPeg.hoveredPI = selectedPegTo.pi;
					currentHoverPeg.isHovering = true;
				}
			}
		}

		else
		{
			if (currentHoverPeg != null)
			{
				currentHoverPeg.isHovering = false;
				currentHoverPeg = null;
			}
		}
	}

	/*
	*   DetectCluePegOnBoard()
	*   From an already selected PegSelector sets an empty Clue Peg into a peg type.
	*
	*   Returns: void
	*/
	void DetectCluePegOnBoard()
	{
		RaycastHit2D pegged = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (pegged.collider != null)
		{
			if (pegged.collider.CompareTag("CluePeg"))
			{
				if (pegged.collider.GetComponent<CluePeg>() != null && !pegged.collider.GetComponent<CluePeg>().ready)
				{
					currentHoverCluePeg = pegged.collider.GetComponent<CluePeg>();
					
					switch(selectedPegTo.currentClue)
					{
						case SelectorPeg.ClueState.RightColor_RightPosition:
							currentHoverCluePeg.hoveredPegColor = selectedPegTo.redCluePeg;
						break;

						case SelectorPeg.ClueState.RightColor_WrongPosition:
							currentHoverCluePeg.hoveredPegColor = selectedPegTo.whiteCluePeg;
						break;
					}

					currentHoverCluePeg.isHovering = true;
				}
			}
		}

		else
		{
			if (currentHoverCluePeg != null)
			{
				currentHoverCluePeg.isHovering = false;
				currentHoverCluePeg = null;
			}
		}
	}

	/*
	*   CheckCurrentRow()
	*   Checks the turn's row in order to determine if its a valid row of Pegs or not
	*
	*   Return: void
	*/
	public void CheckCurrentRow()
	{
		List<int> tmpMaster = masterCollection.GetRowCode();
		List<int> tmpCurrent = rows[usingCurrentRow].GetRowCode();

		string tmpMasters = "";
		string tmpCurrents = "";

		for (int i = 0; i < tmpMaster.Count; i++)
			tmpMasters += tmpMaster[i].ToString();
			
		for (int i = 0; i < tmpCurrent.Count; i++)
			tmpCurrents += tmpCurrent[i].ToString();
		
		if (selectedPegTo != null)
			DeSelectAPeg();
		
		/*Debug.Log("Master Row: " + tmpMasters);
		Debug.Log("Current Row: " + tmpCurrents);*/

		if (currentCodemaker == CodemakerIs.Player && currentCodebreaker == CodebreakerIs.Player)
		{
			if (rows[usingCurrentRow].CheckForAvailableTurn() && tmpMasters == tmpCurrents)
			{
				usingCurrentRow = 10;
				canPlay = false;
				winText.text = "THE CODEBREAKER, " + codebreakerName.ToUpper() + ", IS THE MASTERMIND!";
				winsCodebreaker++;
				readyMakerButton.SetBool("show", false);
				readyBreakerButton.SetBool("show", false);
				ShowFinalPegs();
				currentTurn = Turn.Empty;
				SaveGameProgress();
				music.changeToWin = true;
			}

			else if (rows[usingCurrentRow].CheckForAvailableTurn() && currentTurn == Turn.Codemaker)
			{
				usingCurrentRow++;

				if (usingCurrentRow == 4)
					music.changeToControl = true;

				else if (usingCurrentRow == 9)
					music.changeToAnticipation = true;

				else if (usingCurrentRow == 10)
				{
					canPlay = false;
					currentTurn = Turn.Empty;
					winText.text = "THE CODEMAKER, " + codemakerName.ToUpper() + ", IS THE MASTERMIND!";
					readyMakerButton.SetBool("show", false);
					winsCodemaker++;
					SaveGameProgress();
					readyBreakerButton.SetBool("show", false);
					ShowFinalPegs();
					music.changeToWin = true;
				}

				if (usingCurrentRow != 10)
				{
					currentTurn = Turn.Codebreaker;

					if (finalPegs.GetBool("show"))
						finalPegs.SetBool("show", false);

					readyBreakerButton.SetBool("show", true);
					readyMakerButton.SetBool("show", false);
					CheckTurnRows();
				}
			}

			else if (rows[usingCurrentRow].CheckForAvailableTurn() && currentTurn == Turn.Codebreaker && canPlay)
			{
				currentTurn = Turn.Codemaker;
				readyBreakerButton.SetBool("show", false);
				readyMakerButton.SetBool("show", true);
			}

			else if (!rows[usingCurrentRow].CheckForAvailableTurn())
				music.PlaySoundEffect(invalidRowSFX);
		}

		else if (currentCodemaker == CodemakerIs.AI && currentCodebreaker == CodebreakerIs.Player)
		{
			if (rows[usingCurrentRow].CheckForAvailableTurn() && tmpMasters == tmpCurrents)
			{
				usingCurrentRow = 10;
				winText.text = "THE CODEBREAKER, " + codebreakerName.ToUpper() + ", IS THE MASTERMIND!";
				winsCodebreaker++;
				SaveGameProgress();
				currentTurn = Turn.Empty;
				canPlay = false;

				for (int i = 0; i < rows[usingCurrentRow].pegsInThisSpace.Count; i++)
				{	
					if (masterCollection.GetRowCode().Contains(rows[usingCurrentRow].pegsInThisSpace[i].pegCodeValue))
					{
						if (rows[usingCurrentRow].pegsInThisSpace[i].pegCodeValue == masterCollection.pegsInThisSpace[i].pegCodeValue)
							if (codeMakerClueMode == CodemakerAIClues.Normal)
								StartCoroutine(SetCluePegAfterTime("RCRP", rows[usingCurrentRow].cluePegsInThisSpace[i]));
							else
								StartCoroutine(SetCluePegAfterTime("RCWP", rows[usingCurrentRow].cluePegsInThisSpace[i]));

						else
							if (codeMakerClueMode == CodemakerAIClues.Normal)
								StartCoroutine(SetCluePegAfterTime("RCWP", rows[usingCurrentRow].cluePegsInThisSpace[i]));
							else
								StartCoroutine(SetCluePegAfterTime("RCRP", rows[usingCurrentRow].cluePegsInThisSpace[i]));
					}

					else
						StartCoroutine(SetCluePegAfterTime("E", rows[usingCurrentRow].cluePegsInThisSpace[i]));
				}

				ShowFinalPegs();
				readyMakerButton.SetBool("show", false);
				readyBreakerButton.SetBool("show", false);
				music.changeToWin = true;
			}

			else if (rows[usingCurrentRow].CheckForAvailableTurn() && currentTurn == Turn.Codebreaker)
			{
				for (int i = 0; i < rows[usingCurrentRow].pegsInThisSpace.Count; i++)
				{	
					if (masterCollection.GetRowCode().Contains(rows[usingCurrentRow].pegsInThisSpace[i].pegCodeValue))
					{
						if (rows[usingCurrentRow].pegsInThisSpace[i].pegCodeValue == masterCollection.pegsInThisSpace[i].pegCodeValue)
							if (codeMakerClueMode == CodemakerAIClues.Normal)
								StartCoroutine(SetCluePegAfterTime("RCRP", rows[usingCurrentRow].cluePegsInThisSpace[i]));
							else
								StartCoroutine(SetCluePegAfterTime("RCWP", rows[usingCurrentRow].cluePegsInThisSpace[i]));

						else
							if (codeMakerClueMode == CodemakerAIClues.Normal)
								StartCoroutine(SetCluePegAfterTime("RCWP", rows[usingCurrentRow].cluePegsInThisSpace[i]));
							else
								StartCoroutine(SetCluePegAfterTime("RCRP", rows[usingCurrentRow].cluePegsInThisSpace[i]));
					}

					else
						StartCoroutine(SetCluePegAfterTime("E", rows[usingCurrentRow].cluePegsInThisSpace[i]));
				}

				usingCurrentRow++;

				if (usingCurrentRow == 4)
					music.changeToControl = true;
				
				else if (usingCurrentRow == 9)
					music.changeToAnticipation = true;
					
				else if (usingCurrentRow == 10)
				{
					canPlay = false;
					currentTurn = Turn.Empty;
					winText.text = "THE CODEMAKER, " + codemakerName.ToUpper() + ", IS THE MASTERMIND!";
					winsCodemaker++;
					SaveGameProgress();
					readyMakerButton.SetBool("show", false);
					readyBreakerButton.SetBool("show", false);
					music.changeToWin = true;
					ShowFinalPegs();
				}

				if (usingCurrentRow != 10)
				{
					currentTurn = Turn.Codebreaker;

					if (finalPegs.GetBool("show"))
						finalPegs.SetBool("show", false);

					CheckTurnRows();
				}
			}

			else if (!rows[usingCurrentRow].CheckForAvailableTurn())
				music.PlaySoundEffect(invalidRowSFX);
		}
	}

	/*
	*	ChangeScene(string sceneName)
	*	Starts a coroutine to load another scene
	*
	*	Arguments: sceneName - The name of the scene to load
	*
	*	Returns: void
	*/
	public void ChangeScene	(string sceneName)
	{
		transitionStart.SetBool("go", true);
		StartCoroutine(LoadASScene(sceneName));
	}

	/*
	*	LoadASScene(string _sceneName)
	*	Coroutine that loads asyncronously a scene, it pre loads the scene
	*
	*	Returns: coroutine
	*/
	IEnumerator LoadASScene(string _sceneName)
    {
        yield return new WaitForSeconds(2f);

        AsyncOperation op = SceneManager.LoadSceneAsync(_sceneName);

        while (!op.isDone)
        {
            yield return null;
        }
    }

	/*
	*	TriggerConfetti()
	*	Triggers the confetti for the win in order to be timed with the music
	*
	*	Returns: void
	*/
	public void TriggerConfetti() 
	{
		confettiWin.SetActive(true);
		goToMenuButton.SetActive(true);
	}

	/*
	*	SetCluePegAfterTime(string caseClue)
	*	Makes a little delay for the clue pegs put by the AI
	*
	*	Arguments - caseClue: Clue Peg type
	*			  - cluePegTo: The Clue Peg to set
	*
	*	Returns: coroutine
	*/
	IEnumerator SetCluePegAfterTime(string caseClue, CluePeg cluePegTo)
	{
		yield return new WaitForSeconds(timeBtwClues);

		switch (caseClue)
		{
			case "RCRP":
				cluePegTo.SetCluePeg(CluePeg.ClueState.RightColor_RightPosition);
				cluePegTo.isEmpty = false;
				cluePegTo.sr.color = Color.Lerp(cluePegTo.sr.color, Color.black, .5f);
			break;

			case "RCWP":
				cluePegTo.SetCluePeg(CluePeg.ClueState.RightColor_WrongPosition);
				cluePegTo.isEmpty = false;
				cluePegTo.sr.color = Color.Lerp(cluePegTo.sr.color, Color.black, .5f);
			break;

			case "E":
				cluePegTo.SetCluePeg(CluePeg.ClueState.Empty);
				cluePegTo.isEmpty = false;
				cluePegTo.sr.color = Color.Lerp(cluePegTo.sr.color, Color.black, .5f);
			break;
		}
	}

	/*
	*	SetReferencePegs()
	*	Duplicates the Master Code in order to be shown later
	*
	*	Returns: void
	*/
	public void SetReferencePegs()
	{
		if (codemakerChoose.Count <= 0)
			Debug.LogError("No StartPegs passed from the start.");
		
		else
		{
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].referenceCollection)
				{
					for (int j = 0; j < rows[i].pegsInThisSpace.Count; j++)
					{	
						if (rows[i].pegsInThisSpace[j].gameObject.activeSelf)
							if (currentCodebreaker == CodebreakerIs.Player)
								rows[i].pegsInThisSpace[j].SetPegData(codemakerChoose[j].thisPegInfo);
							
							else if (currentCodebreaker == CodebreakerIs.AI)
								rows[i].pegsInThisSpace[j].SetPegData(codemakerChooseVSAI[j].thisPegInfo);
					}

					break;
				}
			}
		}

		canPlay = true;

		startPanel.SetBool("show", true);
		currentTurn = Turn.Codebreaker;
	}

	/*
	*	RemoveStartPanel()
	*	Triggers an animator to remove the first panel of the game
	*
	*	Arguments: _animator - Panel to trigger
	*
	*	Returns void
	*/
	public void RemoveStartPanel(Animator _animator)
	{
		_animator.SetBool("show", true);
		currentTurn = Turn.Codebreaker;
		canPlay = true;
	}

	/*
	*	SetPegLimit()
	*	Sets the board size
	*
	*	Return: void
	*/
	void SetPegLimit()
	{
		for (int i = 0; i < rows.Count; i++)
		{
			for (int j = 0; j < rows[i].pegsInThisSpace.Count; j++)
			{
				if (j >= boardSize)
				{
					rows[i].pegsInThisSpace[j].gameObject.SetActive(false);
					rows[i].cluePegsInThisSpace[j].gameObject.SetActive(false);
				}
			}
		}

		foreach (PegCollection pg in rows)
		{
			int posIndex = -1;

			if (boardSize == 4)
				posIndex = 0;
			
			else if (boardSize == 5)
				posIndex = 1;

			pg.gameObject.transform.position = new Vector2(pg.gameObject.transform.position.x, yValuesForRows[posIndex]);
		}

		for (int i = 0; i < codemakerChoose.Count; i++)
		{
			if (currentCodebreaker == CodebreakerIs.Player)
			{
				if (i >= boardSize)
					codemakerChoose[i].gameObject.SetActive(false);
			}

			else if (currentCodebreaker == CodebreakerIs.AI)
			{
				if (i >= boardSize)
					codemakerChooseVSAI[i].gameObject.SetActive(false);
			}
		}
	}

	/*
	*	ShowFinalPegs()
	*	Triggers an animator that show the Master Code
	*
	*	Returns: void
	*/
	public void ShowFinalPegs()
	{
		bool tmp = finalPegs.GetBool("show");

		finalPegs.SetBool("show", !tmp);
	}

	/*
	*   SelectAPeg()
	*   Shoots a raycast in order to select a PegSelector
	*
	*   Returns: void
	*/
	void SelectAPeg()
	{
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (hit.collider != null)
		{
			if (hit.collider.CompareTag("SelectorPeg"))
			{
				if (hit.collider.GetComponent<SelectorPeg>() != null)
				{
					if (previousSelectedPeg == null)
						previousSelectedPeg = hit.collider.GetComponent<SelectorPeg>();
					else
						previousSelectedPeg = selectedPegTo;

					if (selectedPegTo != null)
						DeSelectAPeg();

					selectedPegTo = hit.collider.GetComponent<SelectorPeg>();

					if (previousSelectedPeg != null)
						previousSelectedPeg.selected = false;

					music.PlaySoundEffect(selectPegSFX);
					selectedPegTo.sg.sortingOrder += 50;
					selectedPegTo.selected = true;
				}
			}
		}
	}

	/*
	*	GenerateMasterCollection()
	*	Used by the AI to randomly generate a Master Code
	*
	*	Returns: void
	*/
	void GenerateMasterCollection()
	{
		if (codeMakerRepeatNumber)
		{
			List<int> randRow = new List<int>();

			for (int i = 0; i < boardSize; i++)
				randRow.Add(Random.Range(0, 9));
			
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].referenceCollection)
				{
					for (int j = 0; j < rows[i].pegsInThisSpace.Count; j++)
					{	
						if (rows[i].pegsInThisSpace[j].gameObject.activeSelf)
						{
							PegInfo randPegInfo = allInfo[randRow[j]];
							rows[i].pegsInThisSpace[j].SetPegData(randPegInfo);
						}
					}

					break;
				}
			}
		}

		else
		{
			List<int> usedOneToNine = new List<int>();
			List<int> unusedOneToNine = new List<int>();

			for (int i = 0; i < 9; i++)
				unusedOneToNine.Add(i);
			
			for (int i = 0; i< boardSize; i++)
			{
				int randIndex = Random.Range(0, unusedOneToNine.Count);

				if (!usedOneToNine.Contains(unusedOneToNine[randIndex]))
				{
					usedOneToNine.Add(unusedOneToNine[randIndex]);
					unusedOneToNine.Remove(unusedOneToNine[randIndex]);
				}
			}

			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].referenceCollection)
				{
					for (int j = 0; j < rows[i].pegsInThisSpace.Count; j++)
					{	
						if (rows[i].pegsInThisSpace[j].gameObject.activeSelf)
						{
							PegInfo randPegInfo = allInfo[usedOneToNine[j]];
							rows[i].pegsInThisSpace[j].SetPegData(randPegInfo);
						}
					}

					break;
				}
			}
		}
	}

	/*
	*   DeSelectAPeg()
	*   Deselects a selected SelectorPeg
	*
	*   Returns: void
	*/
	void DeSelectAPeg()
	{
		if (currentHoverPeg != null)
		{
			currentHoverPeg.isHovering = false;
			currentHoverPeg = null;
		}

		selectedPegTo.sg.sortingOrder -= 50;
		selectedPegTo.selected = false;
		selectedPegTo = null;
	}

	/*
	*   CheckTurnRows()
	*	Checks all rows based the current turn, the other rows get a darker color
	*
	*	Returns: void
	*/
	void CheckTurnRows()
	{
		for (int i = 0; i < rows.Count; i++)
		{
			if (i == usingCurrentRow)
			{
				rows[i].SetLight();

				for (int j = 0; j < rows[i].pegsInThisSpace.Count; j++)
				{
					rows[i].pegsInThisSpace[j].notTurn = true;
					rows[i].cluePegsInThisSpace[j].notTurn = true;
				}
			}

			else
			{	
				if (rows[i].isLight)
				{
					rows[i].SetDark();

					for (int j = 0; j < rows[i].pegsInThisSpace.Count; j++)
					{
						rows[i].pegsInThisSpace[j].notTurn = false;
						rows[i].cluePegsInThisSpace[j].notTurn = false;
					}
				}
			}
		}
	}
}
