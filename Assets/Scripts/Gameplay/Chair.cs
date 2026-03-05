using UnityEngine;

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
        CharacterData character = chair.character;
        
        string json = Resources.Load<TextAsset>("Characters/characters.json").text;

        CharacterPrefabs characters = JsonUtility.FromJson<CharacterPrefabs>(json);

        string path = characters.characters[character.type].clothes[character.clothes].colors[character.color].path;

        Character prefab = Resources.Load<Character>(path);

        Character obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, _characterPivot);

        obj.Setup(character);

        Data = chair;

        Character = obj;
    }



    public void PushIn()
    {
        _animator.SetTrigger("PushIn");
    }

    public void PullOut()
    {
        _animator.SetTrigger("PullOut");
    }



    public void Release()
    {
        Destroy(Character.gameObject);
    }
}
