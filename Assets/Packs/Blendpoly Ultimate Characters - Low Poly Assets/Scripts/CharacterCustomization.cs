using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blendpoly.Character
{
    public class CharacterCustomization : MonoBehaviour
    {
        public GameObject Skin_ref, Clothes_ref, Eyes_ref, Beard_ref, Glasses_ref, Hair_ref, Shoes_ref, Body_ref;
        public int ClotheSkinIndex = 0, EyeandskinColor_Index = 0;

        public Material[] ClothSkins;
        public Material[] EyeColor;
        public Material[] SkinColor;

        public void ChangeAccessory(GameObject BodyPart, int direction)
        {
            int ActiveChildIndex = GetCurrentActiveAccessory(BodyPart);
            int newIndex = (ActiveChildIndex + direction) % BodyPart.transform.childCount;
            if (newIndex < 0) newIndex += BodyPart.transform.childCount;

            BodyPart.transform.GetChild(newIndex).gameObject.SetActive(true);
            BodyPart.transform.GetChild(ActiveChildIndex).gameObject.SetActive(false);
            ClotheSkinIndex = 0;
        }
        public void ChangeColor(GameObject BodyPart, int direction)
        {
            if (BodyPart.name.Contains("Eye"))
            {
                EyeandskinColor_Index = (EyeandskinColor_Index + direction) % EyeColor.Length;
                if (EyeandskinColor_Index < 0) EyeandskinColor_Index += EyeColor.Length;
                BodyPart.GetComponent<SkinnedMeshRenderer>().sharedMaterial = EyeColor[EyeandskinColor_Index];
            }
            else
            {
                EyeandskinColor_Index = (EyeandskinColor_Index + direction) % SkinColor.Length;
                if (EyeandskinColor_Index < 0) EyeandskinColor_Index += SkinColor.Length;
                BodyPart.GetComponent<SkinnedMeshRenderer>().sharedMaterial = SkinColor[EyeandskinColor_Index];
            }
        }
        public void ChangeClothesSkin(GameObject BodyPart, int direction)
        {
            if (BodyPart == null)
            {
                return;
            }

            int activeChildIndex = GetCurrentActiveAccessory(BodyPart);
            if (activeChildIndex == -1)
            {
                return;
            }

            ClotheSkinIndex = (ClotheSkinIndex + direction) % ClothSkins.Length;
            if (ClotheSkinIndex < 0) ClotheSkinIndex += ClothSkins.Length;

            Transform activeChild = BodyPart.transform.GetChild(activeChildIndex);
            SkinnedMeshRenderer renderer = activeChild.GetComponent<SkinnedMeshRenderer>();
            if (renderer == null)
            {
                return;
            }
            renderer.sharedMaterial = ClothSkins[ClotheSkinIndex];
        }
        private int GetCurrentActiveAccessory(GameObject Parent)
        {
            if (Parent == null)
            {
                Debug.LogError("Parent GameObject is null.");
                return -1;
            }

            for (int i = 0; i < Parent.transform.childCount; i++)
            {
                Transform child = Parent.transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    return i;
                }
            }
            // No active children found
            return -1;
        }
    }
}
