using michitai;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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



    private void Awake()
    {
        _uiManager.OnPanelChanged += OnPanelChanged;


        _uiManager.MainMenu.OnPlay += () => _uiManager.Setup(EMainMenuPanel.Matchmakings);


        _uiManager.Matchmakings.OnJoin += (m) => _ = OnJoinMatchmaking(m);

        _uiManager.Matchmakings.OnBack += () => _uiManager.Setup(EMainMenuPanel.MainMenu);

        _uiManager.Matchmakings.OnCreate += () => _ = OnCreateMatchmaking();
        

        _uiManager.Matchmaking.OnBack += () => _uiManager.Setup(EMainMenuPanel.Matchmakings);

        _uiManager.Matchmaking.OnStart += () => _ = OnStartMatchmaking();


        _uiManager.Setup(EMainMenuPanel.MainMenu);


        _armchairController.Setup(SaveLoadManager.Player.character);

        _armchairController.SitDown();
    }



    private async Task OnJoinMatchmaking(MatchmakingLobby matchmaking)
    {
        _isJoiningMatchmaking = true;

        _uiManager.Setup(EMainMenuPanel.Matchmaking);

        // Show joining matchmaking...

        _uiManager.Matchmaking.Setup(false);

        bool success = await Client.JoinMatchmaking(SaveLoadManager.User.PlayerToken, matchmaking.matchmaking_id);

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

        bool success = await Client.CreateMatchmaking(SaveLoadManager.User.PlayerToken);

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

    private async Task OnStartMatchmaking()
    {
        _isStartingMatchmaking = true;

        // Show staring matchmaking...

        bool success = await Client.StartMatchmaking(SaveLoadManager.User.PlayerToken);

        if (success)
        {
            // Preload players data...

            CurrentRoomInfo room = null;

            do
            {
                RoomResponse roomResponse = await Client.CurrentRoom(SaveLoadManager.User.PlayerToken);

                if (!roomResponse.Success)
                {
                    // Show Error Start Matchmaking (Current Room)

                    _uiManager.Setup(EMainMenuPanel.Matchmakings);
                }
            }
            while (room == null);

            // Collect all players custom data and send to others


            // Load Gameplay Scene...
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
        Task<List<MatchmakingLobby>> task = Client.MatchmakingsList();


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
        yield return new WaitUntil(() => !_isCreatingMatchmaking && !_isJoiningMatchmaking);


        Task<MatchmakingInfo<RulesData>> matchmakingTask = Client.CurrentMatchmaking(SaveLoadManager.User.PlayerToken);

        yield return new WaitUntil(() => matchmakingTask.IsCompleted);

        Task<List<MatchmakingPlayer>> playersTask = Client.MatchmakingPlayersList(SaveLoadManager.User.PlayerToken);

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
}
