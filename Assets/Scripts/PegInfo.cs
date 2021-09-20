using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*   class PegInfo, Inherits from ScriptableObject
*   This is a data object, his whole purpouse is just to save data of Pegs
*/
[CreateAssetMenu(fileName = "New Peg", menuName = "Create New Peg")]
public class PegInfo : ScriptableObject
{
    public Color pegColor;
    public Sprite daltonicIconSprite;
    public int codeValue;
}
