using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIPanel : UIPanel
{
    [SerializeField]
    private Button _leaderboard;

    [SerializeField]
    private Button _settings;

    [SerializeField]
    private Button _custom;

    [SerializeField]
    private Button _play;



    public event Action OnLeaderboard;

    public event Action OnSettings;

    public event Action OnCustom;

    public event Action OnPlay;



    private void Awake()
    {
        _leaderboard.onClick.AddListener(() => OnLeaderboard?.Invoke());

        _settings.onClick.AddListener(() => OnSettings?.Invoke());

        _custom.onClick.AddListener(() => OnCustom?.Invoke());

        _play.onClick.AddListener(() => OnPlay?.Invoke());
    }
}
