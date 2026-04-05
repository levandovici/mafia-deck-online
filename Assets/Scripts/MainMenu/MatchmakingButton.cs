using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingButton : Button
{
    [SerializeField]
    private TextMeshProUGUI _text;



    public void Setup(int number, int players, int maxPlayers)
    {
        _text.text = $"Matchmaking {number} - Players: {players}/{maxPlayers}";
    }
}
