using UnityEngine;

public class ArmchairController : MonoBehaviour
{
    [SerializeField]
    private Armchair _armchair;



    public void Setup(CharacterCustomData character)
    {
        _armchair.Setup(character);
    }



    public void SitDown()
    {
        _armchair.SitDown();
    }

    public void StandUp()
    {
        _armchair.StandUp();
    }
}
