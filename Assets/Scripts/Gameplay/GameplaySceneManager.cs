using UnityEngine;

public class GameplaySceneManager : MonoBehaviour
{
    [SerializeField]
    private GameplayUIManager _uiManager;

    [SerializeField]
    private TableController _tableController;



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


        for (int i = 0; i < chairs.Length; i++)
        {
            CharacterData character = new CharacterData(Random.Range(0, typesCount),
                Random.Range(0, clothesCount), Random.Range(0, colorCount));

            chairs[i] = new ChairData(EChairState.PulledOut, character);
        }


        _tableController.Setup(new TableData(tableType, chairs));
    }
}
