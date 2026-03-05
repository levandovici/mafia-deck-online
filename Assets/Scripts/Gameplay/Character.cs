using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private CharacterData _character;



    public CharacterData Data
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



    public void Setup(CharacterData character)
    {
        Data = character;
    }



    public void SitDown()
    {
        _animator.SetTrigger("SitDown");
    }

    public void StandUp()
    {
        _animator.SetTrigger("StandUp");
    }



    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }
}
