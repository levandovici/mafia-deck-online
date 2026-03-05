using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public int type;

    public int clothes;

    public int color;



    public bool IsValid
    {
        get
        {
            return type != -1 && clothes != -1 && color != -1;
        }
    }



    public CharacterData() : this(-1, -1, -1)
    {

    }

    public CharacterData(int type, int clothes, int color)
    {
        this.type = type;

        this.clothes = clothes;
        
        this.color = color;
    }
}
