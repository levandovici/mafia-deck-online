using System;
using UnityEngine;

[Serializable]
public class CharacterCustomData
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



    public CharacterCustomData(int type, int clothes, int color)
    {
        this.type = type;

        this.clothes = clothes;

        this.color = color;
    }

    public CharacterCustomData() : this(0, 0, 0)
    {

    }
}
