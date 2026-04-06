using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using michitai;

public class MatchmakingUIPanel : UIPanel
{
    [SerializeField]
    private MatchmakingPlayerButton _playerButtonPrefab;

    [SerializeField]
    private RectTransform _playerButtonsParent;

    [SerializeField]
    private MatchmakingPlayerButton[] _playerButtons = new MatchmakingPlayerButton[0];


    [SerializeField]
    private UIPanel _informationPanel;

    private MatchmakingPlayer _current = null;

    [SerializeField]
    private TextMeshProUGUI _players;

    [SerializeField]
    private Button _back;

    [SerializeField]
    private Button _start;



    public event Action OnBack;

    public event Action OnStart;



    private void Awake()
    {
        _back.onClick.AddListener(() => OnBack?.Invoke());

        _start.onClick.AddListener(() => OnStart?.Invoke());


        SetupButtons(0);

        _informationPanel.Close();
    }



    public void Setup(bool isHost, int maxPlayers = 6)
    {
        var matchmaking = new MatchmakingInfo<RulesData>();

        matchmaking.current_players = 1;

        matchmaking.max_players = maxPlayers;

        matchmaking.is_host = isHost;


        var player = new MatchmakingPlayer();

        player.is_host = isHost;


        Setup(matchmaking, new List<MatchmakingPlayer> { player });
    }

    public void Setup(MatchmakingInfo<RulesData> matcmaking, List<MatchmakingPlayer> players)
    {
        if (_informationPanel.Opened && _current != null)
        {
            bool contains = false;

            foreach (MatchmakingPlayer player in players)
            {
                if (player.player_id == _current.player_id)
                {
                    contains = true;

                    break;
                }
            }

            if (!contains)
            {
                _current = null;

                _informationPanel.Close();
            }
        }

        SetupButtons(players);

        _players.text = $"Players: {matcmaking.current_players}/{matcmaking.max_players}";

        _start.gameObject.SetActive(matcmaking.is_host);

        _start.interactable = matcmaking.current_players >= matcmaking.max_players;
    }



    private void Setup(MatchmakingPlayer current)
    {
        if (_current != null && _current.player_id == current.player_id)
        {
            _current = null;

            _informationPanel.Close();
        }
        else
        {
            _current = current;

            _informationPanel.Open();
        }
    }



    private void SetupButtons(List<MatchmakingPlayer> players)
    {
        SetupButtons(players.Count);

        for (int i = 0; i < _playerButtons.Length; i++)
        {
            _playerButtons[i].Setup(i + 1, players[i].is_host);

            if (_current != null && players[i].player_id == _current.player_id)
            {
                _playerButtons[i].Select();
            }

            _playerButtons[i].onClick.RemoveAllListeners();

            int index = i;

            _playerButtons[i].onClick.AddListener(() => Setup(players[index]));
        }
    }

    private void SetupButtons(int count)
    {
        if (count < _playerButtons.Length)
        {
            for (int i = _playerButtons.Length - count - 1; i >= 0; i--)
            {
                Destroy(_playerButtons[i].gameObject);
            }

            MatchmakingPlayerButton[] buttons = new MatchmakingPlayerButton[count];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = _playerButtons[i];
            }

            _playerButtons = buttons;
        }
        else if (count > _playerButtons.Length)
        {
            MatchmakingPlayerButton[] buttons = new MatchmakingPlayerButton[count];

            for (int i = 0; i < _playerButtons.Length; i++)
            {
                buttons[i] = _playerButtons[i];
            }

            for (int i = 0; i < count - _playerButtons.Length; i++)
            {
                MatchmakingPlayerButton button = Instantiate(_playerButtonPrefab, _playerButtonsParent);

                buttons[_playerButtons.Length + i] = button;
            }

            _playerButtons = buttons;
        }
    }
}
