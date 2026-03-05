using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public int type;

    public int color;

    public int clothes;



    public bool IsValid
    {
        get
        {
            return type != -1 && color != -1 && clothes != -1;
        }
    }



    public CharacterData() : this(-1, -1, -1)
    {

    }

    public CharacterData(int type, int color, int clothes)
    {
        this.type = type;

        this.color = color;
        
        this.clothes = clothes;
    }
}
