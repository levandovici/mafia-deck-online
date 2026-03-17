using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUIPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Slider _slider;



    public void Setup(float value)
    {
        _text.text = $"Loading... {Mathf.FloorToInt(value * 100f)}%";

        _slider.value = value;
    }
}
