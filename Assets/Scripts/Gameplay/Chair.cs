using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class Chair : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Transform _characterPivot;

    private ChairData _chair;

    [SerializeField]
    private Character _character = null;



    public ChairData Data
    {
        get
        {
            return _chair;
        }

        private set
        {
            _chair = value;
        }
    }

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



    public void Setup(ChairData chair)
    {
        ClearCharacter();

        CharacterData character = chair.character;
        
        string json = Resources.Load<TextAsset>("Characters/characters").text;

        CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(json);

        string path = characters.characters[character.type].clothes[character.clothes].colors[character.color].path;

        Character prefab = Resources.Load<Character>(path);

        Character obj = Instantiate(prefab, _characterPivot, false);

        obj.Setup(character);

        Data = chair;

        Character = obj;
    }



    public void SitDown()
    {
        _animator.SetTrigger("PushIn");

        if (Character != null)
            Character.SitDown();
    }

    public void StandUp()
    {
        _animator.SetTrigger("PullOut");

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
        if(Character != null)
        {
            Destroy(Character.gameObject);

            Character = null;
        }
    }
}
