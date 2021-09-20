using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   class CluePeg, Inherits from MonoBehaviour
*   Handles the CluePeg GameObject
*
*   Functions:
*       Start(), void
*       Update(), void
*       SetCluePeg(ClueState _currentClue), void
*       OnMouseExit(), void
*       DeSetCluePeg(), void
*/
public class CluePeg : MonoBehaviour
{
    public enum ClueState {Empty, RightColor_RightPosition, RightColor_WrongPosition}
    public ClueState currentClue;
    public Sprite pegSprite;
    public SpriteRenderer sr;
    public Sprite ogSprite;
    public bool isEmpty;
    public bool isHovering;
    public bool notTurn;
    public bool ready;
    public bool forceDeactive;
    public Color hoveredPegColor;

    BoxCollider2D bx;

    /*
	*   Awake()
	*   Gets executed before first frame of execution
	*
	*   Returns: void
	*/
    void Awake()
    {
        bx = GetComponent<BoxCollider2D>();
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    /*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
    void Start()
    {
        
    }

    /*
	*   Update()
	*   Gets executed every frame
	*
	*   Returns: void
	*/
    void Update()
    {   
        if (!forceDeactive)
        { 
            bx.enabled = notTurn;

            if (notTurn)
            {
                if (isHovering)
                {
                    sr.color = hoveredPegColor;
                    sr.sprite = pegSprite;
                }

                else if (!isHovering && !isEmpty)
                {
                    switch (currentClue)
                    {
                        case ClueState.Empty: case ClueState.RightColor_WrongPosition:
                            sr.color = Color.white;
                        break;

                        case ClueState.RightColor_RightPosition:
                            sr.color = Color.red;
                        break;
                    }
                }

                if (!isHovering && isEmpty)
                {
                    Color tmp = sr.color;

                    tmp = Color.white;
                    tmp.a = 1f;
                    sr.sprite = ogSprite;

                    sr.color = tmp;
                }
            }
        }
    }

    /*
    *   SetCluePeg(ClueState _currentClue)
    *   Sets the CluePeg sprite and color based on currentClue   
    *
    *   Arguments: _currentClue - Enum to assing the mode
    *   Returns: void
    */
    public void SetCluePeg(ClueState _currentClue)
    {
        currentClue = _currentClue;

        switch (currentClue)
        {
            case ClueState.Empty:
                sr.sprite = ogSprite;
                sr.color = Color.white;
            break;

            case ClueState.RightColor_RightPosition:
                sr.sprite = pegSprite;
                sr.color = Color.red;
            break;

            case ClueState.RightColor_WrongPosition:
                sr.sprite = pegSprite;
                sr.color = Color.white;
            break;
        }
    }

    /*
    *   DeSetCluePeg(ClueState _currentClue)
    *   Resets the selected Clue Peg into his empty states   
    *
    *   Arguments: _currentClue - Enum to assing the mode
    *   Returns: void
    */
    public void DeSetCluePeg()
    {
        currentClue = ClueState.Empty;
        sr.sprite = ogSprite;
        sr.color = Color.white;
    }

    /*
	*	OnMouseExit()
	*	Gets called when the mouse exits the GameObject
	*
	*	Returns: void
	*/
	void OnMouseExit()
	{
		if (!notTurn && isHovering)
			isHovering = false;
	}
}
