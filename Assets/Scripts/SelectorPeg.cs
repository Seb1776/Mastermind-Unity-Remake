using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/*
*   class SelectorPeg, Inherits from MonoBehaviour
*   Handles the SelectorPeg gameobject
*
*   Functions:
*       Start(), void
*       Update(), void
*/
public class SelectorPeg : MonoBehaviour
{
    public PegInfo pi;
    public bool selected;
    public bool regenerate;
    public bool canBeSelected;
    public float restorePositionSpeed;
    public float restoreScaleSpeed;
    public Vector3 originalPosition;
    public SortingGroup sg;
    
    public bool isCluePeg;
    public enum ClueState{RightColor_RightPosition, RightColor_WrongPosition}
    public ClueState currentClue;
    public Color redCluePeg;
    public Color whiteCluePeg;

    float ogScale;
    BoxCollider2D bx;
    SpriteRenderer sr;
    SpriteRenderer daltonicSr;
    GameManager manager;

    /*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
    void Start()
    {
        sg = GetComponent<SortingGroup>();
        bx = GetComponent<BoxCollider2D>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        originalPosition = transform.position;

        ogScale = transform.localScale.x;
        sr = GetComponent<SpriteRenderer>();

        if (!isCluePeg)
            daltonicSr = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (pi != null || !isCluePeg)
        {
            if (manager.daltonicMode)
            {
                daltonicSr.sprite = pi.daltonicIconSprite;
                daltonicSr.color = pi.pegColor;
            }

            else
            {
                daltonicSr.sprite = null;
                daltonicSr.color = Color.black;
            }

            sr.color = pi.pegColor;
        }
    }

    /*
	*   Update()
	*   Gets executed every frame
	*
	*   Returns: void
	*/
    void Update()
    {
        bx.enabled = !selected;

        if ((manager.currentTurn == GameManager.Turn.Codebreaker && !isCluePeg) || 
            (manager.currentTurn == GameManager.Turn.Codemaker && isCluePeg))
        {
            if (selected)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = Vector2.Lerp(transform.position, mousePos, restorePositionSpeed * Time.deltaTime);
            }

            else
            {   
                if (transform.position != originalPosition && canBeSelected)
                    transform.position = Vector2.Lerp(transform.position, originalPosition, restorePositionSpeed * Time.deltaTime);
            }
        }

        if (!isCluePeg)
            daltonicSr.gameObject.SetActive(manager.daltonicMode);
    }
}
