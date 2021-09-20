using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
*   class SetPegStart
*   This class handles the master code pegs
*
*   Functions:
*       Start(), void
*       Update(), void
*       DownArrow(), void
*       UpArrow(), void
*/
public class SetPegStart : MonoBehaviour
{
    public List<PegInfo> allPegs = new List<PegInfo>();
    public int currentColorIndex;
    public Image daltIconImage;
    public PegInfo thisPegInfo;

    GameManager manager;
    Image pegImage;
    int maxColorIndex;

    /*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pegImage = GetComponent<Image>();
        maxColorIndex = allPegs.Count;

        pegImage.color = allPegs[0].pegColor;
        thisPegInfo = allPegs[0];

        if (manager.daltonicMode)
        {
            daltIconImage.sprite = allPegs[0].daltonicIconSprite;
            daltIconImage.color = allPegs[0].pegColor;
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
        daltIconImage.enabled = manager.daltonicMode;
    }

    /*
	*   UpArrow()
	*   Gets the next Color peg for the Master Code
	*
	*   Returns: void
	*/
    public void UpArrow()
    {
        currentColorIndex++;

        if (currentColorIndex >= maxColorIndex)
            currentColorIndex = 0;
        
        pegImage.color = allPegs[currentColorIndex].pegColor;
        thisPegInfo = allPegs[currentColorIndex];

        if (manager.daltonicMode)
        {
            daltIconImage.sprite = allPegs[currentColorIndex].daltonicIconSprite;
            daltIconImage.color = allPegs[currentColorIndex].pegColor;
        }
    }

    /*
	*   UpArrow()
	*   Gets the previous Color peg for the Master Code
	*
	*   Returns: void
	*/
    public void DownArrow()
    {
        currentColorIndex--;

        if (currentColorIndex < 0)
            currentColorIndex = maxColorIndex - 1;
        
        pegImage.color = allPegs[currentColorIndex].pegColor;
        thisPegInfo = allPegs[currentColorIndex];

        if (manager.daltonicMode)
        {
            daltIconImage.sprite = allPegs[currentColorIndex].daltonicIconSprite;
            daltIconImage.color = allPegs[currentColorIndex].pegColor;
        }
    }
}
