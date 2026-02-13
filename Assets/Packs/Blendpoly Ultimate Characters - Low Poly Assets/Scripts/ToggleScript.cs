using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blendpoly.ToggleScript
{
    public class ToggleScript : MonoBehaviour
    {
        [SerializeField] private GameObject onButton;
        [SerializeField] private GameObject offButton;
        [SerializeField] private GameObject BodyPart;
        private int toggleIndex;
        void Start()
        {
            toggleIndex = PlayerPrefs.GetInt("ToggleState", 0);
        }

        void Update()
        {
            PlayerPrefs.SetInt("ToggleState", toggleIndex);
        }
        public void ToggleOn()
        {
            toggleIndex = 1;
            onButton.SetActive(true);
            offButton.SetActive(false);
            BodyPart.SetActive(true);
        }
        public void ToggleOff()
        {
            toggleIndex = 0;
            onButton.SetActive(false);
            offButton.SetActive(true);
            BodyPart.SetActive(false);
        }
    }
}
