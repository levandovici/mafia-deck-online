using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using michitai;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField]
    private MainMenuUIManager _uiManager;

    [SerializeField]
    private ArmchairController _armchairController;

    private Coroutine _matchmakingsCoroutine = null;

    private Coroutine _matchmakingCoroutine = null;

    private bool _isCreatingMatchmaking = false;

    private bool _isJoiningMatchmaking = false;

    private bool _isStartingMatchmaking = false;

    [SerializeField]
    private int _userIndex = -1;



    private void Awake()
    {
        _uiManager.OnPanelChanged += OnPanelChanged;


        _uiManager.MainMenu.OnPlay += () => _uiManager.Setup(EMainMenuPanel.Matchmakings);


        _uiManager.Matchmakings.OnJoin += (m) => _ = OnJoinMatchmaking(m);

        _uiManager.Matchmakings.OnBack += () => _uiManager.Setup(EMainMenuPanel.MainMenu);

        _uiManager.Matchmakings.OnCreate += () => _ = OnCreateMatchmaking();
        

        _uiManager.Matchmaking.OnBack += () => _uiManager.Setup(EMainMenuPanel.Matchmakings);

        _uiManager.Matchmaking.OnStart += () => _ = OnStartMatchmaking(true);


        _uiManager.Setup(EMainMenuPanel.MainMenu);


        _armchairController.Setup(SaveLoadManager.Player.character);

        _armchairController.SitDown();
    }



    private async Task OnJoinMatchmaking(MatchmakingLobby<RulesData> matchmaking)
    {
        _isJoiningMatchmaking = true;

        _uiManager.Setup(EMainMenuPanel.Matchmaking);

        // Show joining matchmaking...

        _uiManager.Matchmaking.Setup(false);

        bool success = await Client.JoinMatchmaking(SaveLoadManager.User.PlayerToken,
            matchmaking.matchmaking_id, SaveLoadManager.Player);

        if(success)
        {
            // Show successful joined 

            _uiManager.Matchmaking.Setup(false);
        }
        else
        {
            // Show Error Join Matchmaking

            _uiManager.Setup(EMainMenuPanel.Matchmakings);
        }

        _isJoiningMatchmaking = false;
    }

    private async Task OnCreateMatchmaking()
    {
        _isCreatingMatchmaking = true;

        _uiManager.Setup(EMainMenuPanel.Matchmaking);

        // Show creating matchmaking...

        _uiManager.Matchmaking.Setup(true);

        bool success = await Client.CreateMatchmaking(SaveLoadManager.User.PlayerToken,
            "Matchmaking", SaveLoadManager.Player, 2);

        if(success)
        {
            // Show Successful created

            _uiManager.Matchmaking.Setup(true);
        }
        else
        {
            // Show Error Create Matchmaking

            _uiManager.Setup(EMainMenuPanel.Matchmakings);
        }

        _isCreatingMatchmaking = false;
    }

    private async Task OnStartMatchmaking(bool isHost)
    {
        _isStartingMatchmaking = true;

        // Show staring matchmaking...

        bool success = isHost ? await Client.StartMatchmaking(SaveLoadManager.User.PlayerToken) : true;

        if (success)
        {
            // Preload players data...

            CurrentRoomInfo<RulesData> room = null;

            do
            {
                RoomResponse roomResponse = await Client.CurrentRoom(SaveLoadManager.User.PlayerToken);

                if (!roomResponse.Success)
                {
                    // Show Error Current Room

                    _uiManager.Setup(EMainMenuPanel.Matchmakings);
                }
                else if(roomResponse.Room != null)
                {
                    room = roomResponse.Room;
                }
            }
            while (room == null);

            // Collect all players custom data

            List<RoomPlayer<PlayerData>> players = await Client.RoomPlayersList(SaveLoadManager.User.PlayerToken);

            SaveLoadManager.CreateGame(new CurrentGameData(room, players));

            // Load Gameplay Scene...

            SaveLoadManager.SceneIndex = 2;

            SceneManager.LoadScene(0);
        }
        else
        {
            // Show Error Start Matchmaking

            _uiManager.Setup(EMainMenuPanel.Matchmakings);
        }

        _isStartingMatchmaking = false;
    }



    private void OnPanelChanged(EMainMenuPanel panel)
    {
        switch (panel)
        {
            case EMainMenuPanel.Matchmakings:
                SetupMatchmakings(); break;

            case EMainMenuPanel.Matchmaking:
                SetupMatchmaking(); break;
        }
    }



    private void SetupMatchmakings()
    {
        if (_matchmakingsCoroutine == null)
        {
            _matchmakingsCoroutine = StartCoroutine(MatchmakingsList());
        }
    }

    private void SetupMatchmaking()
    {
        if(_matchmakingCoroutine == null)
        {
            _matchmakingCoroutine = StartCoroutine(Matchmaking());
        }
    }



    private IEnumerator MatchmakingsList()
    {
        Task<List<MatchmakingLobby<RulesData>>> task = Client.MatchmakingsList();


        yield return new WaitUntil(() => task.IsCompleted);


        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else if(task.Result == null)
        {
            //Show load matchmakings error
        }
        else
        {
            _uiManager.Matchmakings.Setup(task.Result);
        }

        if(_uiManager.Matchmakings.Opened)
        {
            yield return new WaitForSeconds(5f);

            _matchmakingsCoroutine = StartCoroutine(MatchmakingsList());
        }
        else
        {
            _matchmakingsCoroutine = null;
        }
    }

    private IEnumerator Matchmaking()
    {
        yield return new WaitUntil(() => !_isCreatingMatchmaking && !_isJoiningMatchmaking && !_isStartingMatchmaking);


        Task<MatchmakingInfo<RulesData>> matchmakingTask = Client.CurrentMatchmaking(SaveLoadManager.User.PlayerToken);

        yield return new WaitUntil(() => matchmakingTask.IsCompleted);

        Task<List<MatchmakingPlayer<PlayerData>>> playersTask = Client.MatchmakingPlayersList(SaveLoadManager.User.PlayerToken);

        yield return new WaitUntil(() => playersTask.IsCompleted);


        if (matchmakingTask.Exception != null)
        {
            Debug.LogError(matchmakingTask.Exception);
        }
        else if(playersTask.Exception != null)
        {
            Debug.LogError(playersTask.Exception);
        }
        else if (matchmakingTask.Result == null || playersTask.Result == null)
        {
            //Show error
        }
        else
        {
            _uiManager.Matchmaking.Setup(matchmakingTask.Result, playersTask.Result);

            if(matchmakingTask.Result.is_started)
            {
                _ = OnStartMatchmaking(false);

                _matchmakingCoroutine = null;

                // Show loading circle...

                yield break;
            }
        }

        if (_uiManager.Matchmaking.Opened)
        {
            yield return new WaitForSeconds(5f);

            _matchmakingCoroutine = StartCoroutine(Matchmaking());
        }
        else
        {
            _matchmakingCoroutine = null;
        }
    }



    [ContextMenu("SwitchUser")]
    private async Task SwitchUser()
    {
        if (!SaveLoadManager.LoadUser(_userIndex) || !SaveLoadManager.User.IsValid)
        {
            string playerToken = await Client.Register();

            if (playerToken == null)
            {
                //Show Register Error
            }
            else
            {
                SaveLoadManager.CreateUser(new UserData(playerToken));
            }
        }

        AuthResponse auth = await Client.Auth(SaveLoadManager.User.PlayerToken);

        if (auth.Success)
        {
            SaveLoadManager.UploadPlayer(auth.Player);
        }
        else
        {
            //Show Auth Error
        }
    }
}
