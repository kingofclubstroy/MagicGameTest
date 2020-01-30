using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Armor
{
    
    //Name that will be used for the file name
    public string name;
    public List<ArmorPiece> armorPieces;
    

    public Armor(string nameStr)
    {
        name = nameStr;
        armorPieces = new List<ArmorPiece>();

    }

    public void AddArmorPiece(ArmorPiece piece)
    {
        armorPieces.Add(piece);
    }
}
