using Michitai;
using Michitai.Multiplayer;
using Michitai.Multiplayer.Rooms;
using Michitai.Multiplayer.Rooms.Actions;
using Michitai.Multiplayer.Rooms.Updates;
using System.Collections.Generic;
using UnityEngine;

public class CurrentGameData
{
    [SerializeField]
    private CurrentRoomInfo<RulesData> _currentRoom;

    [SerializeField]
    private List<RoomPlayer<PlayerData>> _roomPlayers;



    public CurrentRoomInfo<RulesData> CurrentRoom
    {
        get
        {
            return _currentRoom;
        }

        set
        {
            _currentRoom = value;
        }
    }

    public List<RoomPlayer<PlayerData>> RoomPlayers
    {
        get
        {
            return _roomPlayers;
        }

        set
        {
            _roomPlayers = value;
        }
    }



    public CurrentGameData(CurrentRoomInfo<RulesData> currentRoom, List<RoomPlayer<PlayerData>> roomPlayers)
    {
        _currentRoom = currentRoom;

        _roomPlayers = roomPlayers;
    }
}
