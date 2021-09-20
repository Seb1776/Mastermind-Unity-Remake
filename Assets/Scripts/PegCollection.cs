using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
*   class Peg, Inherits from MonoBehaviour
*   Handles all the Pegs in each row
*
*   Functions:
*       Awake(), void
*       Start(), void
*       CheckForAvailableTurn(), bool
*       SetLight(), void
*       SetDark(), void
*/
public class PegCollection : MonoBehaviour
{
    public List<Peg> pegsInThisSpace = new List<Peg>();
    public List<CluePeg> cluePegsInThisSpace = new List<CluePeg>();
    public bool referenceCollection;
    [Range(-1, 8)]
    public List<int> rowCode = new List<int>();
    public bool isLight;

    GameManager manager;

    /*
	*   Awake()
	*   Gets executed before first frame of execution
	*
	*   Returns: void
	*/
	void Awake()
	{
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

    /*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
    void Start()
    {
        if (!referenceCollection)
            SetDark();
    }

    /*
    *   CheckForAvailableTurn()
    *   Checks if all the pegs have a peg data and converts this row into a valid one
    *
    *   Returns:
    *       true: If all Pegs have data
    *       false: If at least one Peg doesn't has data
    */
    public bool CheckForAvailableTurn()
    {
        int fullPegs = 0;

        if (manager.boardSize != 6)
        {
            int actualPegs = 0;

            for (int i = 0; i < pegsInThisSpace.Count; i++)
                if (pegsInThisSpace[i].gameObject.activeSelf)
                    actualPegs++;
            
            for (int i = 0; i < actualPegs; i++)
                if (!pegsInThisSpace[i].isEmpty)
                    fullPegs++;
            
            if (fullPegs == actualPegs)
                return true;
        }

        else
        {
            for (int i = 0; i < pegsInThisSpace.Count; i++)
                if (!pegsInThisSpace[i].isEmpty)
                    fullPegs++;

            if (fullPegs == pegsInThisSpace.Count)
                return true;
        }

        return false;
    }

    public List<int> GetRowCode()
    {   
        List<int> tmpCode = new List<int>();

        for (int i = 0; i < pegsInThisSpace.Count; i++)
            if (pegsInThisSpace[i].gameObject.activeSelf)
                tmpCode.Add(pegsInThisSpace[i].pegCodeValue);

        return tmpCode;
    }

    /*
	*	SetLight()
	*	Sets the Pegs color to his original tone
	*
	*	Returns: void
	*/
	public void SetLight()
	{
        foreach (Peg peg in pegsInThisSpace)
        {
            peg.sr.color = Color.white;
            peg.daltIconSr.color = Color.white;
        }

        foreach (CluePeg peg in cluePegsInThisSpace)
            peg.sr.color = Color.white;

        isLight = true;
	}

	/*
	*	SetDark()
	*	Sets the Pegs color to a darker tone
	*
	*	Returns: void
	*/
	public void SetDark()
	{
        foreach (Peg peg in pegsInThisSpace)
        {
            peg.sr.color = Color.Lerp(peg.sr.color, Color.black, .5f);
            peg.daltIconSr.color = Color.Lerp(peg.sr.color, Color.black, .5f);
        }

        foreach (CluePeg peg in cluePegsInThisSpace)
            peg.sr.color = Color.Lerp(peg.sr.color, Color.black, .5f);

        isLight = false;
	}
}
