using System;
using UnityEngine;

[Serializable]
public class ChairData
{
    public EChairState state;

    public bool localPlayer;

    public CharacterData character;



    public bool IsValid
    {
        get
        {
            return state != EChairState.None && character != null;
        }
    }



    public ChairData(bool playerChair) : this(EChairState.PulledOut, playerChair, new CharacterData())
    {

    }

    public ChairData(EChairState state, bool localPlayer, CharacterData character = null)
    {
        this.state = state;

        this.localPlayer = localPlayer;

        this.character = character;
    }
}

public enum EChairState
{
    None, PushedIn, PulledOut, PushingIn, PullingOut
}
