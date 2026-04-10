using System;
using UnityEngine;

[Serializable]
public class TableData
{
    public ETableType type;

    public ChairData[] chairs;

    public int localPlayer;



    public bool IsValid
    {
        get
        {
            switch(type)
            {
                case ETableType.Players_6:
                    return chairs != null && chairs.Length == 6;

                case ETableType.Players_8:
                    return chairs != null && chairs.Length == 8;

                case ETableType.Players_10:
                    return chairs != null && chairs.Length == 10;

                case ETableType.Players_12:
                    return chairs != null && chairs.Length == 12;

                default:
                    return false;
            }
        }
    }



    public TableData(int localPlayer) : this(ETableType.Players_6, new ChairData[6], localPlayer)
    {

    }

    public TableData(ETableType type, ChairData[] chairs, int localPlayer)
    {
        this.type = type;

        this.chairs = chairs;

        this.localPlayer = localPlayer;
    }
}

public enum ETableType
{
    None, Players_6, Players_8, Players_10, Players_12
}
