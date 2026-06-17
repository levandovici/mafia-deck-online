using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Michitai;
using System;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField]
    private LoadingUIManager _uiManger;



    private async void Awake()
    {
        await Initialise();

        Load();
    }



    private async Task Initialise()
    {
        if(!SaveLoadManager.LoadUser() || !SaveLoadManager.User.IsValid)
        {
           string playerToken = await MultiplayerClient.Register();

            if(playerToken == null)
            {
                //Show Register Error
            }
            else
            {
                SaveLoadManager.CreateUser(new UserData(playerToken));
            }
        }

        AuthResponse auth = await MultiplayerClient.Auth(SaveLoadManager.User.PlayerToken);

        if(auth.Success)
        {
            SaveLoadManager.UploadPlayer(auth.Player);
        }
        else
        {
            //Show Auth Error
        }
    }

    private void Load()
    {
        _uiManger.Setup(ELoadingPanel.Loading);

        StartCoroutine(Loading(SaveLoadManager.SceneIndex));
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
