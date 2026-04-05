using System;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField]
    private MainMenuUIPanel _mainMenu;

    [SerializeField]
    private MatchmakingsUIPanel _matchmakings;

    [SerializeField]
    private MatchmakingUIPanel _matchmaking;

    [SerializeField]
    private CustomUIPanel _custom;

    [SerializeField]
    private EMainMenuPanel _current = EMainMenuPanel.None;



    public event Action<EMainMenuPanel> OnPanelChanged;



    public MainMenuUIPanel MainMenu
    {
        get
        {
            return _mainMenu;
        }
    }

    public MatchmakingsUIPanel Matchmakings
    {
        get
        {
            return _matchmakings;
        }
    }

    public MatchmakingUIPanel Matchmaking
    {
        get
        {
            return _matchmaking;
        }
    }

    public CustomUIPanel Custom
    {
        get
        {
            return _custom;
        }
    }

    public EMainMenuPanel Current
    {
        get
        {
            return _current;
        }

        private set
        {
            _current = value;
        }
    }



    public void Setup(EMainMenuPanel panel)
    {
        Change(panel);
    }



    private void Change(EMainMenuPanel panel)
    {
        if (Current == panel)
            return;

        Close(Current);

        Open(panel);

        Current = panel;

        OnPanelChanged?.Invoke(panel);
    }


    private void Open(EMainMenuPanel panel)
    {
        switch(panel)
        {
            case EMainMenuPanel.MainMenu:
                _mainMenu.Open(); break;

            case EMainMenuPanel.Matchmakings:
                _matchmakings.Open(); break;

            case EMainMenuPanel.Matchmaking:
                _matchmaking.Open(); break;

            case EMainMenuPanel.Custom:
                _custom.Open(); break;
        }
    }

    private void Close(EMainMenuPanel panel)
    {
        switch (panel)
        {
            case EMainMenuPanel.MainMenu:
                _mainMenu.Close(); break;

            case EMainMenuPanel.Matchmakings:
                _matchmakings.Close(); break;

            case EMainMenuPanel.Matchmaking:
                _matchmaking.Close(); break;

            case EMainMenuPanel.Custom:
                _custom.Close(); break;
        }
    }
}

public enum EMainMenuPanel
{
    None, MainMenu, Matchmakings, Matchmaking, Custom
}
