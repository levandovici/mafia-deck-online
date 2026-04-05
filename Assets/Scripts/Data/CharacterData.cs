using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public CharacterCustomData custom;



    public bool IsValid
    {
        get
        {
            return custom.IsValid;
        }
    }



    public CharacterData() : this(new CharacterCustomData())
    {

    }

    public CharacterData(CharacterCustomData custom)
    {
        this.custom = custom;
    }
}
