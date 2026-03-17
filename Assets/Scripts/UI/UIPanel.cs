using UnityEngine;

public class UIPanel : MonoBehaviour
{
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
