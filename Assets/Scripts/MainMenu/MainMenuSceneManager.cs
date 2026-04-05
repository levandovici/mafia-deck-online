using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField]
    private MainMenuUIManager _uiManager;

    [SerializeField]
    private ArmchairController _armchairController;



    private void Awake()
    {
        _armchairController.Setup(SaveLoadManager.Player.character);

        _armchairController.SitDown();
    }
}
