using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mafia
{
    public class SpeechBubbleManager : MonoBehaviour
    {
        public static SpeechBubbleManager Instance { get; private set; }

        [SerializeField]
        private GameObject _speechBubblePrefab;

        [SerializeField]
        private Canvas _targetCanvas;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ShowSpeechBubble(Transform targetHead, string text, Color backgroundColor, string speakerName = "")
        {
            if (_speechBubblePrefab == null || targetHead == null)
            {
                // Fallback: Create dynamic speech bubble if prefab is null
                CreateDynamicSpeechBubble(targetHead, text, backgroundColor, speakerName);
                return;
            }

            GameObject bubble = Instantiate(_speechBubblePrefab, _targetCanvas != null ? _targetCanvas.transform : transform);
            StartCoroutine(AnimateBubble(bubble, targetHead, text, backgroundColor, speakerName));
        }

        private void CreateDynamicSpeechBubble(Transform targetHead, string text, Color backgroundColor, string speakerName)
        {
            Canvas parentCanvas = _targetCanvas;
            if (parentCanvas == null) parentCanvas = FindAnyObjectByType<Canvas>();
            if (parentCanvas == null) return;

            GameObject bubbleGo = new GameObject("SpeechBubble", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            bubbleGo.transform.SetParent(parentCanvas.transform, false);

            Image img = bubbleGo.GetComponent<Image>();
            img.color = backgroundColor;

            // Content text
            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(bubbleGo.transform, false);
            TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = string.IsNullOrEmpty(speakerName) ? text : $"<b>{speakerName}:</b> {text}";
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            RectTransform rect = bubbleGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(240, 70);

            RectTransform textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            StartCoroutine(AnimateBubble(bubbleGo, targetHead, text, backgroundColor, speakerName));
        }

        private IEnumerator AnimateBubble(GameObject bubble, Transform targetHead, string text, Color backgroundColor, string speakerName)
        {
            CanvasGroup group = bubble.GetComponent<CanvasGroup>();
            if (group == null) group = bubble.AddComponent<CanvasGroup>();

            RectTransform rect = bubble.GetComponent<RectTransform>();
            Camera cam = Camera.main;

            float duration = 3.5f;
            float elapsed = 0f;

            Vector3 baseOffset = new Vector3(0, 2.2f, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                if (targetHead != null && cam != null)
                {
                    Vector3 worldPos = targetHead.position + baseOffset + new Vector3(0, Mathf.Sin(elapsed * 2f) * 0.05f, 0);
                    Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

                    if (screenPos.z > 0)
                    {
                        rect.position = screenPos;
                    }
                }

                // Fade out near end
                if (elapsed > duration - 0.75f)
                {
                    group.alpha = Mathf.Lerp(1f, 0f, (elapsed - (duration - 0.75f)) / 0.75f);
                }
                else
                {
                    group.alpha = Mathf.Min(1f, elapsed * 4f);
                }

                yield return null;
            }

            Destroy(bubble);
        }
    }
}
