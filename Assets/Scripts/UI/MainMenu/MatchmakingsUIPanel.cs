using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Michitai;
using Michitai.Multiplayer;
using Michitai.Multiplayer.Matchmaking;
using System.Collections.Generic;

public class MatchmakingsUIPanel : UIPanel
{
    [SerializeField]
    private MatchmakingButton _matchmakingButtonPrefab;

    [SerializeField]
    private RectTransform _matchmakingButtonsParent;

    [SerializeField]
    private MatchmakingButton[] _matchmakingButtons = new MatchmakingButton[0];


    [SerializeField]
    private UIPanel _informationPanel;

    private MatchmakingLobby<RulesData> _current = null;

    [SerializeField]
    private TextMeshProUGUI _players;

    [SerializeField]
    private Button _join;

    [SerializeField]
    private Button _back;

    [SerializeField]
    private Button _create;



    public event Action<MatchmakingLobby<RulesData>> OnJoin;

    public event Action OnBack;

    public event Action OnCreate;



    private void Awake()
    {
        _join.onClick.AddListener(() => OnJoin?.Invoke(_current));

        _back.onClick.AddListener(() => OnBack?.Invoke());

        _create.onClick.AddListener(() => OnCreate?.Invoke());


        SetupButtons(0);

        _informationPanel.Close();
    }



    public void Setup(List<MatchmakingLobby<RulesData>> matchmakings)
    {
        if (_informationPanel.Opened && _current != null)
        {
            bool contains = false;

            foreach (MatchmakingLobby<RulesData> lobby in matchmakings)
            {
                if (lobby.matchmaking_id == _current.matchmaking_id)
                {
                    contains = true;

                    break;
                }
            }

            if(!contains)
            {
                _current = null;

                _informationPanel.Close();
            }
        }

        SetupButtons(matchmakings);
    }



    private void Setup(MatchmakingLobby<RulesData> current)
    {
        if (_current != null && _current.matchmaking_id == current.matchmaking_id)
        {
            _current = null;

            _informationPanel.Close();
        }
        else
        {
            _current = current;

            _players.text = $"Players: {current.current_players}/{current.max_players}";

            _informationPanel.Open();
        }
    }



    private void SetupButtons(List<MatchmakingLobby<RulesData>> matchmakings)
    {
        SetupButtons(matchmakings.Count);

        for(int i = 0; i < _matchmakingButtons.Length; i++)
        {
            _matchmakingButtons[i].Setup(i + 1, matchmakings[i].current_players, matchmakings[i].max_players);

            if (_current != null && matchmakings[i].matchmaking_id == _current.matchmaking_id)
            {
                _matchmakingButtons[i].Select();
            }

            _matchmakingButtons[i].onClick.RemoveAllListeners();

            int index = i;

            _matchmakingButtons[i].onClick.AddListener(() => Setup(matchmakings[index]));
        }
    }

    private void SetupButtons(int count)
    {
        if (count < _matchmakingButtons.Length)
        {
            for (int i = _matchmakingButtons.Length - count - 1; i >= 0; i--)
            {
                Destroy(_matchmakingButtons[i].gameObject);
            }

            MatchmakingButton[] buttons = new MatchmakingButton[count];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = _matchmakingButtons[i];
            }

            _matchmakingButtons = buttons;
        }
        else if (count > _matchmakingButtons.Length)
        {
            MatchmakingButton[] buttons = new MatchmakingButton[count];

            for (int i = 0; i < _matchmakingButtons.Length; i++)
            {
                buttons[i] = _matchmakingButtons[i];
            }

            for (int i = 0; i < count - _matchmakingButtons.Length; i++)
            {
                MatchmakingButton button = Instantiate(_matchmakingButtonPrefab, _matchmakingButtonsParent);

                buttons[_matchmakingButtons.Length + i] = button;
            }

            _matchmakingButtons = buttons;
        }
    }
}
