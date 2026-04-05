using UnityEngine;

public class Armchair : MonoBehaviour
{
    [SerializeField]
    private Transform _characterPivot;

    [SerializeField]
    private Character _character = null;



    public Character Character
    {
        get
        {
            return _character;
        }

        private set
        {
            _character = value;
        }
    }



    public void Setup(CharacterCustomData custom)
    {
        ClearCharacter();

        string json = Resources.Load<TextAsset>("Characters/characters").text;

        CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(json);

        string path = characters.characters[custom.type].clothes[custom.clothes].colors[custom.color].path;

        Character prefab = Resources.Load<Character>(path);

        Character obj = Instantiate(prefab, _characterPivot, false);

        Character = obj;
    }



    public void SitDown()
    {
        if (Character != null)
            Character.SitDown();
    }

    public void StandUp()
    {
        if (Character != null)
            Character.StandUp();
    }



    public void Release()
    {
        Destroy(Character.gameObject);

        Character = null;
    }



    private void ClearCharacter()
    {
        if (Character != null)
        {
            Destroy(Character.gameObject);

            Character = null;
        }
    }
}
