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



    private void Awake()
    {
        _uiManager.OnPanelChanged += OnPanelChanged;


        _uiManager.MainMenu.OnPlay += () => _uiManager.Setup(EMainMenuPanel.Matchmakings);


        _uiManager.Matchmakings.OnBack += () => _uiManager.Setup(EMainMenuPanel.MainMenu);

        _uiManager.Matchmakings.OnCreate += () => _ = Client.CreateMatchmaking(SaveLoadManager.User.PlayerToken);


        _uiManager.Setup(EMainMenuPanel.MainMenu);


        _armchairController.Setup(SaveLoadManager.Player.character);

        _armchairController.SitDown();
    }


    private void OnPanelChanged(EMainMenuPanel panel)
    {
        switch(panel)
        {
            case EMainMenuPanel.Matchmakings:
                SetupMatchmakings(); break;
        }
    }



    private void SetupMatchmakings()
    {
        if (_matchmakingsCoroutine == null)
        {
            _matchmakingsCoroutine = StartCoroutine(MatchmakingsList());
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
}
