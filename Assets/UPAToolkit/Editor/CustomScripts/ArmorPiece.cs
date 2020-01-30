using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmorPiece
{
    
    public enum color
    {
        Primary,
        Secondary,
        Tertiary
    }

    public string name;

    public ArmorPiece(string nameStr) {

        name = nameStr;


    }


}
