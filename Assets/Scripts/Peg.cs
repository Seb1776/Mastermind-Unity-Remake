using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   class Peg, Inherits from MonoBehaviour
*   Handles the Peg GameObject
*
*   Functions:
*		Awake(), void
*       Update(), void
*       SetPegData(PegInfo pi), void
*       DeSetPegData(), void
*		OnMouseExit(), void
*/
public class Peg : MonoBehaviour
{
	public Sprite pegSprite;
	public Sprite emptyPegSprite;
	public bool isEmpty;
	public bool isHovering;
	public bool ready;
	public bool notTurn;
	public int pegCodeValue;
	public SpriteRenderer sr;
	public PegInfo hoveredPI;
	public PegInfo setPI;
	public SpriteRenderer daltIconSr;

	BoxCollider2D bx;
	GameManager manager;

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
		daltIconSr = transform.GetChild(1).GetComponent<SpriteRenderer>();
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

	/*
	*   Update()
	*   Gets executed every frame
	*
	*   Returns: void
	*/
	void Update()
	{   
		bx.enabled = notTurn;

		if (notTurn)
		{
			if (isEmpty)
				pegCodeValue = -1;

			if (isHovering)
			{
				Color tmp = sr.color;

				tmp.a = .5f;
				tmp.r = hoveredPI.pegColor.r;
				tmp.g = hoveredPI.pegColor.g;
				tmp.b = hoveredPI.pegColor.b;
				sr.sprite = pegSprite; 

				if (manager.daltonicMode)
					daltIconSr.sprite = hoveredPI.daltonicIconSprite;

				daltIconSr.color = tmp;
				sr.color = tmp;
			}

			else if (!isHovering && isEmpty)
			{
				Color tmp = sr.color;

				tmp = Color.white;
				sr.sprite = emptyPegSprite;

				sr.color = tmp;

				daltIconSr.sprite = null;
			}

			else if (!isHovering && !isEmpty)
			{
				sr.color = setPI.pegColor;

				if (manager.daltonicMode)
					daltIconSr.sprite = setPI.daltonicIconSprite;

				daltIconSr.color = setPI.pegColor;
			}
		}
	}

	/*
	*   SetPegData(PegInfo pi)
	*   Sets the Peg sprite and color based on the PegInfo
	*   
	*   Arguments: pi - PegInfo variable
	*   Returns: void
	*/
	public void SetPegData(PegInfo pi)
	{
		if (manager.daltonicMode)
		{
			daltIconSr.sprite = pi.daltonicIconSprite;
			daltIconSr.color = pi.pegColor;
		}

		else
		{
			daltIconSr.sprite = null;
			daltIconSr.color = Color.black;
		}
		
		hoveredPI = pi;
		sr.sprite = pegSprite;
		sr.color = pi.pegColor;
		pegCodeValue = pi.codeValue;
		
		isEmpty = false;
	}

	/*
	*   DeSetPegData()
	*   Desets all the Peg data, restores it to an empty peg
	*
	*   Returns: void
	*/
	public void DeSetPegData()
	{
		sr.sprite = emptyPegSprite;
		sr.color = Color.white;
		daltIconSr.sprite = null;
		daltIconSr.color = Color.black;
		pegCodeValue = -1;
		hoveredPI = null;
		isEmpty = true;
	}

	/*
	*	OnMouseExit()
	*	Gets called when the mouse exits the GameObject
	*
	*	Returns: void
	*/
	void OnMouseExit()
	{
		if (!ready && isHovering)
			isHovering = false;
	}
}
