using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Blendpoly.Character;

namespace Blendpoly.UI
{
    public class CustomizationButtonListener : MonoBehaviour
    {
        //Hair
        [Header("Hair buttons")]
        [Header("Hair Buttons (Mesh Changer)")]
        [SerializeField] private Button HairButton_Previous;
        [SerializeField] private Button HairButton_Next;
        [Header("Hair Buttons (Color Changer)")]
        [SerializeField] private Button HairColorButton_Previous;
        [SerializeField] private Button HairColorButton_Next;

        //Eyes
        [Header("Eyes buttons")]
        [Header("Hair Buttons (Color Changer)")]
        [SerializeField] private Button EyesButton_Previous;
        [SerializeField] private Button EyesButton_Next;

        //Beard
        [Header("Beard buttons")]
        [Header("Beard Buttons (Mesh Changer)")]
        [SerializeField] private Button BeardButton_Previous;
        [SerializeField] private Button BeardButton_Next;
        [Header("Beard Buttons (Color Changer)")]
        [SerializeField] private Button BeardColorButton_Previous;
        [SerializeField] private Button BeardColorButton_Next;

        //Clothes
        [Header("Clothes buttons")]
        [Header("Clothes Buttons (Mesh Changer)")]
        [SerializeField] private Button ClothesButton_Previous;
        [SerializeField] private Button ClothesButton_Next;
        [Header("Clothes Buttons (Color Changer)")]
        [SerializeField] private Button ClothesColorButton_Previous;
        [SerializeField] private Button ClothesColorButton_Next;

        //Glasses
        [Header("Glasses buttons")]
        [Header("Glasses Buttons (Mesh Changer)")]
        [SerializeField] private Button GlassesButton_Previous;
        [SerializeField] private Button GlassesButton_Next;

        //Shoes
        [Header("Shoes buttons")]
        [Header("Shoes Buttons (Mesh Changer)")]
        [SerializeField] private Button ShoesButton_Previous;
        [SerializeField] private Button ShoesButton_Next;
        [Header("Shoes Buttons (Color Changer)")]
        [SerializeField] private Button ShoesColorButton_Previous;
        [SerializeField] private Button ShoesColorButton_Next;

        //Body
        [Header("Body buttons")]
        [Header("Body Buttons (Color Changer)")]
        [SerializeField] private Button BodyColorButton_Previous;
        [SerializeField] private Button BodyColorButton_Next;

        [SerializeField] private CharacterCustomization CharacterCustom_Ref;

        private void Awake()
        {
            ///////Change BodyParts///////

            //-- Hair Buttons --//

            HairButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Hair_ref, -1); });
            HairButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Hair_ref, 1); });

            //-- Eyes Buttons --//

            EyesButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeColor(CharacterCustom_Ref.Eyes_ref, -1); });
            EyesButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeColor(CharacterCustom_Ref.Eyes_ref, 1); });

            //-- Clothing Buttons --//

            ClothesButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Clothes_ref, -1); });
            ClothesButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Clothes_ref, 1); });

            //-- Beard Buttons --//

            BeardButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Beard_ref, -1); });
            BeardButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Beard_ref, 1); });

            //-- Glasses Buttons --//

            GlassesButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Glasses_ref, -1); });
            GlassesButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Glasses_ref, 1); });

            //-- Shoes Buttons --//

            ShoesButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Shoes_ref, -1); });
            ShoesButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeAccessory(CharacterCustom_Ref.Shoes_ref, 1); });

            //-- Body Buttons --//

            BodyColorButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeColor(CharacterCustom_Ref.Body_ref, -1); });
            BodyColorButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeColor(CharacterCustom_Ref.Body_ref, 1); });

            ///////Change BodyParts Colors///////

            //-- Hair Buttons --//

            HairColorButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Hair_ref, -1); });
            HairColorButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Hair_ref, 1); });

            //-- Beard Buttons --//

            BeardColorButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Beard_ref, -1); });
            BeardColorButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Beard_ref, 1); });

            //-- Clothes Buttons --//

            ClothesColorButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Clothes_ref, -1); });
            ClothesColorButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Clothes_ref, 1); });

            //-- Shoes Buttons --//

            ShoesColorButton_Previous.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Shoes_ref, -1); });
            ShoesColorButton_Next.onClick.AddListener(() => { CharacterCustom_Ref.ChangeClothesSkin(CharacterCustom_Ref.Shoes_ref, 1); });
        }
    }
}

