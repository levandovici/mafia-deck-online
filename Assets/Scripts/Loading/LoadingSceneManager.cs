using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField]
    private LoadingUIManager _uiManger;



    private void Start()
    {
        Load();
    }



    private void Load()
    {
        _uiManger.Setup(ELoadingPanel.Loading);

        StartCoroutine(Loading(SaveLoadManager.Instance.SceneIndex));
    }



    private IEnumerator Loading(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            _uiManger.Loading.Setup(operation.progress);

            yield return null;
        }
    }
}
