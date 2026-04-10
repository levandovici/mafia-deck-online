using UnityEngine;

public class GameplaySceneManager : MonoBehaviour
{
    [SerializeField]
    private GameplayUIManager _uiManager;

    [SerializeField]
    private TableController _tableController;

    [SerializeField]
    private CameraController _cameraController;



    private void Awake()
    {
        ChairData[] chairs = new ChairData[6];

        string json = Resources.Load<TextAsset>("Characters/characters").text;

        CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(json);

        int typesCount = characters.characters.Count;

        int clothesCount = characters.characters[0].clothes.Count;

        int colorCount = characters.characters[0].clothes[0].colors.Count;


        int localPlayer = -1;

        for (int i = 0; i < chairs.Length; i++)
        {
            if (i < SaveLoadManager.CurrentGame.RoomPlayers.Count)
            {
                if (SaveLoadManager.CurrentGame.RoomPlayers[i].is_local)
                {
                    localPlayer = i;
                }

                if (SaveLoadManager.CurrentGame.RoomPlayers[i].PlayerData != null &&
                SaveLoadManager.CurrentGame.RoomPlayers[i].PlayerData.character != null)
                {
                    CharacterData character =
                        new CharacterData(SaveLoadManager.CurrentGame.RoomPlayers[i].PlayerData.character);

                    chairs[i] = new ChairData(EChairState.PulledOut, SaveLoadManager.CurrentGame.RoomPlayers[i].is_local, character);
                }
                else
                {
                    chairs[i] = new ChairData(EChairState.PulledOut, SaveLoadManager.CurrentGame.RoomPlayers[i].is_local, null);
                }
            }
            else
            {
                chairs[i] = new ChairData(EChairState.PulledOut, false, null);
            }
        }


        _tableController.Setup(new TableData(ETableType.Players_6, chairs, localPlayer));

        _cameraController.Setup(_tableController.Table.CameraPoints, localPlayer);
    }

    private void Start()
    {
        _tableController.Table.SitDownAll();
    }



    [ContextMenu("Generate")]
    private void Generate()
    {
        int tableIndex = Random.Range(0, 4);

        ETableType tableType = ETableType.None;

        ChairData[] chairs = null;

        switch(tableIndex)
        {
            case 0:
                tableType = ETableType.Players_6;

                chairs = new ChairData[6];
                break;

            case 1:
                tableType = ETableType.Players_8;

                chairs = new ChairData[8];
                break;

            case 2:
                tableType = ETableType.Players_10;

                chairs = new ChairData[10];
                break;

            case 3:
                tableType = ETableType.Players_12;

                chairs = new ChairData[12];
                break;
        }


        string json = Resources.Load<TextAsset>("Characters/characters").text;

        CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(json);

        int typesCount = characters.characters.Count;

        int clothesCount = characters.characters[0].clothes.Count;

        int colorCount = characters.characters[0].clothes[0].colors.Count;


        int localPlayer = Random.Range(0, chairs.Length);

        for (int i = 0; i < chairs.Length; i++)
        {
            CharacterData character = new CharacterData(new CharacterCustomData(Random.Range(0, typesCount),
                Random.Range(0, clothesCount), Random.Range(0, colorCount)));

            chairs[i] = new ChairData(EChairState.PulledOut, i == localPlayer, character);
        }


        _tableController.Setup(new TableData(tableType, chairs, localPlayer));

        _cameraController.Setup(_tableController.Table.CameraPoints, localPlayer);
    }
}
