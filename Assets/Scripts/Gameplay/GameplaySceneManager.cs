using UnityEngine;
using System.Collections.Generic;
using Michitai.Multiplayer.Rooms;
using Mafia;

public class GameplaySceneManager : MonoBehaviour
{
    [SerializeField]
    private GameplayUIManager _uiManager;

    [SerializeField]
    private TableController _tableController;

    [SerializeField]
    private CameraController _cameraController;

    private List<RoomPlayer<PlayerData>> _roomPlayers = new List<RoomPlayer<PlayerData>>();
    private PlayerData _localPlayerData;

    private void Awake()
    {
        TextAsset charJson = Resources.Load<TextAsset>("Characters/characters");
        CharacterPrefabs characters = charJson != null ? JsonUtility.FromJson<CharacterPrefabs>(charJson.text) : null;

        int typesCount = (characters != null && characters.characters != null) ? characters.characters.Count : 1;

        ChairData[] chairs = new ChairData[6];
        int localPlayer = 0;

        bool hasSavedGame = SaveLoadManager.CurrentGame != null && SaveLoadManager.CurrentGame.RoomPlayers != null && SaveLoadManager.CurrentGame.RoomPlayers.Count > 0;

        _roomPlayers.Clear();

        for (int i = 0; i < 6; i++)
        {
            if (hasSavedGame && i < SaveLoadManager.CurrentGame.RoomPlayers.Count)
            {
                var rp = SaveLoadManager.CurrentGame.RoomPlayers[i];
                _roomPlayers.Add(rp);

                if (rp.is_local)
                {
                    localPlayer = i;
                    _localPlayerData = rp.PlayerData;
                }

                if (rp.PlayerData != null && rp.PlayerData.character != null)
                {
                    CharacterData character = new CharacterData(rp.PlayerData.character);
                    chairs[i] = new ChairData(EChairState.PulledOut, rp.is_local, character);
                }
                else
                {
                    chairs[i] = new ChairData(EChairState.PulledOut, rp.is_local, CreateRandomCharacterData(i, typesCount, characters));
                }
            }
            else
            {
                // Fallback / Bot player for standalone testing
                bool isLocal = !hasSavedGame && (i == 0);
                if (isLocal) localPlayer = i;

                CharacterCustomData customData = new CharacterCustomData();
                customData.type = i % typesCount;
                if (characters != null && characters.characters[customData.type].clothes.Count > 0)
                {
                    customData.clothes = Random.Range(0, characters.characters[customData.type].clothes.Count);
                    if (characters.characters[customData.type].clothes[customData.clothes].colors.Count > 0)
                    {
                        customData.color = Random.Range(0, characters.characters[customData.type].clothes[customData.clothes].colors.Count);
                    }
                }

                PlayerData pData = new PlayerData();
                pData.character = customData;

                if (isLocal) _localPlayerData = pData;

                RoomPlayer<PlayerData> botPlayer = new RoomPlayer<PlayerData>
                {
                    player_id = i + 1,
                    player_name = isLocal ? "Игрок (Вы)" : $"Игрок {i + 1}",
                    is_local = isLocal
                };
                botPlayer.SetPlayerData(pData);

                _roomPlayers.Add(botPlayer);
                chairs[i] = new ChairData(EChairState.PulledOut, isLocal, new CharacterData(customData));
            }
        }

        if (_tableController == null) _tableController = GetComponent<TableController>() ?? FindAnyObjectByType<TableController>();
        if (_cameraController == null) _cameraController = GetComponent<CameraController>() ?? FindAnyObjectByType<CameraController>();

        _tableController.Setup(new TableData(ETableType.Players_6, chairs, localPlayer));
        _cameraController.Setup(_tableController.Table.CameraPoints, localPlayer);
    }

    private CharacterData CreateRandomCharacterData(int index, int typesCount, CharacterPrefabs characters)
    {
        CharacterCustomData customData = new CharacterCustomData();
        customData.type = index % typesCount;
        if (characters != null && characters.characters[customData.type].clothes.Count > 0)
        {
            customData.clothes = Random.Range(0, characters.characters[customData.type].clothes.Count);
            if (characters.characters[customData.type].clothes[customData.clothes].colors.Count > 0)
            {
                customData.color = Random.Range(0, characters.characters[customData.type].clothes[customData.clothes].colors.Count);
            }
        }
        return new CharacterData(customData);
    }

    private void Start()
    {
        _tableController.Table.SitDownAll();

        if (MafiaGameManager.Instance != null)
        {
            MafiaGameManager.Instance.InitializeGame(_tableController, _roomPlayers, _localPlayerData);
        }
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

        TextAsset charAsset = Resources.Load<TextAsset>("Characters/characters");
        if (charAsset == null) return;

        CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(charAsset.text);
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
