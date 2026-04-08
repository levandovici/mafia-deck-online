using UnityEngine;

public static class SaveLoadManager
{
    private const string _userDataKey = "UserData";



    private static int _sceneIndex = 1;

    private static UserData _userData = null;

    private static PlayerData _playerData = null;

    private static CurrentGameData _currentGameData = null;



    public static int SceneIndex
    {
        get
        {
            return _sceneIndex;
        }

        set
        {
            _sceneIndex = value;
        }
    }

    public static UserData User
    {
        get
        {
            return _userData;
        }

        private set
        {
            _userData = value;
        }
    }

    public static PlayerData Player
    {
        get
        {
            return _playerData;
        }

        private set
        {
            _playerData = value;
        }
    }

    public static CurrentGameData CurrentGame
    {
        get
        {
            return _currentGameData;
        }

        private set
        {
            _currentGameData = value;
        }
    }



    public static void SaveUser()
    {
        if (User == null)
            return;

        string json = JsonUtility.ToJson(User);

        PlayerPrefs.SetString(_userDataKey, json);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// returns true if loaded, returns false if not loaded
    /// </summary>
    /// <returns></returns>
    public static bool LoadUser()
    {
        if (!PlayerPrefs.HasKey(_userDataKey))
            return false;

        try
        {
            string json = PlayerPrefs.GetString(_userDataKey);

            UserData user = JsonUtility.FromJson<UserData>(json);

            User = user;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void CreateUser(UserData user)
    {
        User = user;

        SaveUser();
    }


    public static void UploadPlayer(PlayerData player)
    {
        Player = player;
    }



    public static void CreateGame(CurrentGameData currentGame)
    {
        CurrentGame = currentGame;
    }
}