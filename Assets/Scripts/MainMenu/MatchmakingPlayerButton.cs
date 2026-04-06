using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingPlayerButton : Button
{
    [SerializeField]
    private TextMeshProUGUI _text;



    public void Setup(int number, bool isHost)
    {
        if (isHost)
        {
            _text.text = $"{number} - Host Player";
        }
        else
        {
            _text.text = $"{number} - Player";
        }
    }
}
