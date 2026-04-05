using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public bool Opened
    {
        get
        {
            return gameObject.activeSelf;
        }
    }



    public void Open()
    {
        SetActive(true);
    }

    public void Close()
    {
        SetActive(false);
    }



    private void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
