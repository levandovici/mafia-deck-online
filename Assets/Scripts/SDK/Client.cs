using UnityEngine;
using michitai;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public static class Client
{
    private static GameSDK _client = new GameSDK("5280c806cfcba3d7ab823663f5b490ac4b29",
        "7e41fd31b472d84a2795442ece78270324f7", logger: new ConsoleLogger());



    public static GameSDK Game
    {
        get
        {
            return _client;
        }
    }



    public static async Task<string> Register()
    {
        PlayerRegisterResponse response = await Game.RegisterPlayer("Player", new PlayerData());

        if (response.success)
        {
            return response.private_key;
        }

        return null;
    }

    public static async Task<AuthResponse> Auth(string playerToken)
    {
        PlayerAuthResponse<PlayerData> response = await Game.AuthenticatePlayer<PlayerData>(playerToken);

        if (response.success)
        {
            PlayerData player = response.player.PlayerData;

            return new AuthResponse(response.success, player ?? new PlayerData());
        }
        else return new AuthResponse(false);
    }


    public static async Task<List<MatchmakingLobby<RulesData>>> MatchmakingsList()
    {
        MatchmakingListResponse<RulesData> response = await Game.GetMatchmakingLobbiesAsync<RulesData>();

        if (response.success)
        {
            return response.lobbies;
        }
        else return null;
    }

    public static async Task<bool> CreateMatchmaking(string playerToken, string matchmakingName, PlayerData playerData, int players)
    {
        MatchmakingCreateResponse response = await Game.CreateMatchmakingLobbyAsync<PlayerData, RulesData>(playerToken, matchmakingName, players, true, false, false, playerData);

        return response.success;
    }

    public static async Task<bool> JoinMatchmaking(string playerToken, string matchmakingId, PlayerData playerData)
    {
        MatchmakingDirectJoinResponse response = await Game.JoinMatchmakingDirectlyAsync<PlayerData>(playerToken, matchmakingId, playerData);

        return response.success;
    }

    public static async Task<MatchmakingInfo<RulesData>> CurrentMatchmaking(string playerToken)
    {
        MatchmakingCurrentResponse<RulesData> response = await Game.GetCurrentMatchmakingStatusAsync<RulesData>(playerToken);

        if (response.success)// && response.in_matchmaking)
        {
            return response.matchmaking;
        }
        else return null;
    }

    public static async Task<List<MatchmakingPlayer<PlayerData>>> MatchmakingPlayersList(string playerToken)
    {
        MatchmakingPlayersResponse<PlayerData> response = await Game.GetMatchmakingPlayersAsync<PlayerData>(playerToken);

        if (response.success)
        {
            return response.players;
        }
        else return null;
    }

    public static async Task<bool> StartMatchmaking(string playerToken)
    {
        MatchmakingStartResponse response = await Game.StartGameFromMatchmakingAsync(playerToken);

        return response.success;
    }

    public static async Task<RoomResponse> CurrentRoom(string playerToken)
    {
        CurrentRoomResponse<RulesData> response = await Game.GetCurrentRoomAsync<RulesData>(playerToken);

        if (response.success)
        {
            return new RoomResponse(true, response.in_room ? response.room : null);
        }
        else return new RoomResponse(false);
    }

    public static async Task<List<RoomPlayer<PlayerData>>> RoomPlayersList(string playerToken)
    {
        RoomPlayersResponse<PlayerData> response = await Game.GetRoomPlayersAsync<PlayerData>(playerToken);

        if (response.success)
        {
            return response.players;
        }
        else return null;
    }
}

public class AuthResponse
{
    [SerializeField]
    private bool _success;
    [SerializeField]
    private PlayerData _player;


    public bool Success
    {
        get
        {
            return _success;
        }

        private set
        {
            _success = value;
        }
    }

    public PlayerData Player
    {
        get
        {
            return _player;
        }

        private set
        {
            _player = value;
        }
    }



    public AuthResponse(bool success, PlayerData player = null)
    {
        Success = success;

        Player = player;
    }
}

public class RoomResponse
{
    [SerializeField]
    private bool _success;
    [SerializeField]
    private CurrentRoomInfo<RulesData> _room;



    public bool Success
    {
        get
        {
            return _success;
        }

        private set
        {
            _success = value;
        }
    }

    public CurrentRoomInfo<RulesData> Room
    {
        get
        {
            return _room;
        }

        private set
        {
            _room = value;
        }
    }



    public RoomResponse(bool success, CurrentRoomInfo<RulesData> room = null)
    {
        Success = success;

        Room = room;
    }
}

[Serializable]
public class RulesData
{

}