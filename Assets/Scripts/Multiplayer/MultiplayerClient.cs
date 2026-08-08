using UnityEngine;
using Michitai;
using Michitai.Multiplayer;
using Michitai.Multiplayer.Players;
using Michitai.Multiplayer.Matchmaking;
using Michitai.Multiplayer.Matchmaking.Requests;
using Michitai.Multiplayer.Rooms;
using Michitai.Multiplayer.Rooms.Actions;
using Michitai.Multiplayer.Rooms.Updates;
using Michitai.Multiplayer.Rooms.Realtime;
using Michitai.Multiplayer.Errors;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public static class MultiplayerClient
{
    private static Client _client = new Client("5bd92edc72e3da572035eac23fa5283083cc",
        "367bc1585718c1fc6b795f4561afd4db30c1", logger: new ConsoleLogger());



    public static Client Client
    {
        get
        {
            return _client;
        }
    }



    public static async Task<string> Register()
    {
        PlayerRegisterResponse response = await Players.RegisterPlayer<PlayerData>(Client, "Player", new PlayerData());

        if (response.success)
        {
            return response.private_key;
        }

        return null;
    }

    public static async Task<AuthResponse> Auth(string playerToken)
    {
        PlayerAuthResponse<PlayerData> response = await Players.AuthenticatePlayer<PlayerData>(Client, playerToken);

        if (response.success)
        {
            PlayerData player = response.player.PlayerData;

            return new AuthResponse(response.success, player ?? new PlayerData());
        }
        else return new AuthResponse(false);
    }


    public static async Task<List<MatchmakingLobby<RulesData>>> MatchmakingsList()
    {
        MatchmakingListResponse<RulesData> response = await Matchmaking.GetMatchmakingLobbiesAsync<RulesData>(Client);

        if (response.success)
        {
            return response.lobbies;
        }
        else return null;
    }

    public static async Task<bool> CreateMatchmaking(string playerToken, string matchmakingName, PlayerData playerData, int players)
    {
        MatchmakingCreateResponse response = await Matchmaking.CreateMatchmakingLobbyAsync<PlayerData, RulesData>(Client, playerToken,
            matchmakingName, players, true, false, false, false, null, playerData, new RulesData());

        return response.success;
    }

    public static async Task<bool> JoinMatchmaking(string playerToken, string matchmakingId, PlayerData playerData)
    {
        MatchmakingDirectJoinResponse response = await Matchmaking.JoinMatchmakingDirectlyAsync<PlayerData>(Client, playerToken, matchmakingId, playerData);

        return response.success;
    }

    public static async Task<MatchmakingInfo<RulesData>> CurrentMatchmaking(string playerToken)
    {
        MatchmakingCurrentResponse<RulesData> response = await Matchmaking.GetCurrentMatchmakingStatusAsync<RulesData>(Client, playerToken);

        if (response.success)// && response.in_matchmaking)
        {
            return response.matchmaking;
        }
        else return null;
    }

    public static async Task<List<MatchmakingPlayer<PlayerData>>> MatchmakingPlayersList(string playerToken)
    {
        MatchmakingPlayersResponse<PlayerData> response = await Matchmaking.GetMatchmakingPlayersAsync<PlayerData>(Client, playerToken);

        if (response.success)
        {
            return response.players;
        }
        else return null;
    }

    public static async Task<bool> StartMatchmaking(string playerToken)
    {
        MatchmakingStartResponse response = await Matchmaking.StartGameFromMatchmakingAsync(Client, playerToken);

        return response.success;
    }

    public static async Task<RoomResponse> CurrentRoom(string playerToken)
    {
        CurrentRoomResponse<RulesData> response = await Rooms.GetCurrentRoomAsync<RulesData>(Client, playerToken);

        if (response.success)
        {
            return new RoomResponse(true, response.in_room ? response.room : null);
        }
        else return new RoomResponse(false);
    }

    public static async Task<List<RoomPlayer<PlayerData>>> RoomPlayersList(string playerToken)
    {
        RoomPlayersResponse<PlayerData> response = await Rooms.GetRoomPlayersAsync<PlayerData>(Client, playerToken);

        if (response.success)
        {
            return response.players;
        }
        else return null;
    }

    public static async Task<bool> SendRoomUpdate<T>(string playerToken, string type, T data, ERoomTargetPlayers target = ERoomTargetPlayers.All, int[] targetIds = null) where T : class, new()
    {
        UpdatePlayersResponse response = await Updates.UpdatePlayersAsync(Client, playerToken, new UpdatePlayers<T>(target, type, data, targetIds));
        return response != null && response.success;
    }

    public static async Task<List<PlayerUpdate>> PollRoomUpdates(string playerToken, ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.All, string lastUpdateId = null)
    {
        PollUpdatesResponse response = await Updates.PollUpdatesAsync(Client, playerToken, new PollUpdates(fromPlayers, null, lastUpdateId));
        if (response != null && response.success)
        {
            return response.updates;
        }
        return null;
    }

    public static async Task<bool> SubmitRoomAction<T>(string playerToken, string actionType, T data, ERoomTargetPlayers target = ERoomTargetPlayers.Host, int[] targetIds = null) where T : class, new()
    {
        ActionSubmitResponse response = await Actions.SubmitActionAsync(Client, playerToken, new SubmitAction<T>(target, actionType, data, targetIds));
        return response != null && response.success;
    }

    public static async Task<List<ActionInfo>> PollRoomActions(string playerToken)
    {
        ActionPollResponse response = await Actions.PollActionsAsync(Client, playerToken);
        if (response != null && response.success)
        {
            return response.actions;
        }
        return null;
    }

    public static async Task<TokenResponse> GetRealtimeToken(string playerToken)
    {
        return await Realtime.GetTokenAsync(Client, playerToken);
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