using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager _Instance = null;



    public static SaveLoadManager Instance
    {
        get
        {
            return _Instance;
        }

        private set
        {
            _Instance = value;
        }
    }



    private int _sceneIndex = 1;



    public int SceneIndex
    {
        get
        {
            return _sceneIndex;
        }

        set
        {
            _sceneIndex = value;
        }
    }



    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);

            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }
}
