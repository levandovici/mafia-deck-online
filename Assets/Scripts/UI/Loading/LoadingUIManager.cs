using UnityEngine;

public class LoadingUIManager : MonoBehaviour
{
    [SerializeField]
    private LoadingUIPanel _loading;

    [SerializeField]
    private ELoadingPanel _current = ELoadingPanel.None;



    public ELoadingPanel Current
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



    public LoadingUIPanel Loading
    {
        get
        {
            return _loading;
        }
    }



    public void Setup(ELoadingPanel panel)
    {
        Change(panel);
    }



    private void Change(ELoadingPanel panel)
    {
        if (Current == panel)
            return;

        Close(Current);

        Open(panel);

        Current = panel;
    }



    private void Open(ELoadingPanel panel)
    {
        switch(panel)
        {
            case ELoadingPanel.Loading:
                _loading.Open();
                break;
        }
    }

    private void Close(ELoadingPanel panel)
    {
        switch (panel)
        {
            case ELoadingPanel.Loading:
                _loading.Close();
                break;
        }
    }
}

public enum ELoadingPanel
{
    None, Loading,
}
