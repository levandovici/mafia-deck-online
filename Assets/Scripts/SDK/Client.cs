using UnityEngine;
using michitai;
using System.Threading.Tasks;

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