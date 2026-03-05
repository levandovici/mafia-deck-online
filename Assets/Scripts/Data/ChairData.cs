using System;
using UnityEngine;

[Serializable]
public class ChairData
{
    public EChairState state;

    public CharacterData character;



    public bool IsValid
    {
        get
        {
            return state != EChairState.None;
        }
    }



    public ChairData() : this(EChairState.PulledOut, new CharacterData())
    {

    }

    public ChairData(EChairState state, CharacterData character)
    {
        this.state = state;

        this.character = character;
    }
}

public enum EChairState
{
    None, PushedIn, PulledOut, PushingIn, PullingOut
}
