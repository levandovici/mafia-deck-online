using UnityEngine;

public static class SaveLoadManager
{
    private static int _sceneIndex = 1;



    public static int SceneIndex
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
}