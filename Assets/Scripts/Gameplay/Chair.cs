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
        Release();

        CharacterData character = chair.character;

        if (character != null)
        {
            string json = Resources.Load<TextAsset>("Characters/characters").text;

            CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(json);

            string path = characters.characters[character.custom.type].clothes[character.custom.clothes].colors[character.custom.color].path;

            Character prefab = Resources.Load<Character>(path);

            Character obj = Instantiate(prefab, _characterPivot, false);

            obj.Setup(character);

            Character = obj;
        }

        Data = chair;
    }



    public void SitDown()
    {
        if (Character == null)
            return;

        _animator.SetTrigger("PushIn");

        Character.SitDown();
    }

    public void StandUp()
    {
        if (Character == null)
            return;

        _animator.SetTrigger("PullOut");

        Character.StandUp();
    }



    public void Release()
    {
        if(Character != null)
        {
            Destroy(Character.gameObject);

            Character = null;
        }
    }
}
