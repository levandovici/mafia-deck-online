using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mafia.UI
{
    public class TopBarUIPanel : UIPanel
    {
        [SerializeField]
        private TextMeshProUGUI _phaseText;

        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private Transform _playerBadgesContainer;

        [SerializeField]
        private GameObject _playerBadgePrefab;

        private List<PlayerBadgeUI> _badges = new List<PlayerBadgeUI>();

        public void UpdatePhase(EGamePhase phase, int round)
        {
            if (_phaseText == null) return;

            switch (phase)
            {
                case EGamePhase.RoleReveal:
                    _phaseText.text = "ЗНАКОМСТВО С РОЛЬЮ";
                    break;
                case EGamePhase.DayDiscussion:
                    _phaseText.text = $"ДЕНЬ {round} — ОБСУЖДЕНИЕ";
                    break;
                case EGamePhase.DayVoting:
                    _phaseText.text = $"ДЕНЬ {round} — ГОЛОСОВАНИЕ";
                    break;
                case EGamePhase.NightPhase:
                    _phaseText.text = $"НОЧЬ {round}";
                    break;
                case EGamePhase.GameOver:
                    _phaseText.text = "ИГРА ОКОНЧЕНА";
                    break;
            }
        }

        public void UpdateTimer(float remainingSeconds)
        {
            if (_timerText == null) return;
            int secs = Mathf.Max(0, Mathf.CeilToInt(remainingSeconds));
            _timerText.text = $"{secs:00}";
        }

        public void SetupPlayerBadges(List<MafiaPlayerState> players)
        {
            // Clear existing badges
            foreach (var badge in _badges)
            {
                if (badge != null && badge.gameObject != null)
                {
                    Destroy(badge.gameObject);
                }
            }
            _badges.Clear();

            if (_playerBadgesContainer == null) return;

            foreach (var p in players)
            {
                GameObject badgeGo = null;
                if (_playerBadgePrefab != null)
                {
                    badgeGo = Instantiate(_playerBadgePrefab, _playerBadgesContainer);
                }
                else
                {
                    badgeGo = CreateDynamicBadgeGO(p, _playerBadgesContainer);
                }

                PlayerBadgeUI badge = badgeGo.GetComponent<PlayerBadgeUI>();
                if (badge == null) badge = badgeGo.AddComponent<PlayerBadgeUI>();

                badge.Setup(p);
                _badges.Add(badge);
            }
        }

        public void UpdatePlayerStates(List<MafiaPlayerState> players)
        {
            foreach (var p in players)
            {
                var badge = _badges.Find(b => b.ChairIndex == p.ChairIndex);
                if (badge != null)
                {
                    badge.UpdateState(p);
                }
            }
        }

        private GameObject CreateDynamicBadgeGO(MafiaPlayerState player, Transform parent)
        {
            GameObject container = new GameObject($"Badge_{player.ChairIndex}", typeof(RectTransform), typeof(Image));
            container.transform.SetParent(parent, false);

            Image bg = container.GetComponent<Image>();
            bg.color = player.IsLocal ? new Color(0.2f, 0.5f, 0.8f, 0.85f) : new Color(0.2f, 0.2f, 0.2f, 0.75f);

            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(container.transform, false);

            TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = $"#{player.ChairIndex + 1}\n{player.Name}";
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            RectTransform textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;

            RectTransform rect = container.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(65, 45);

            return container;
        }
    }

    public class PlayerBadgeUI : MonoBehaviour
    {
        public int ChairIndex { get; private set; }

        private Image _bgImage;
        private TextMeshProUGUI _text;

        public void Setup(MafiaPlayerState player)
        {
            ChairIndex = player.ChairIndex;
            _bgImage = GetComponent<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            UpdateState(player);
        }

        public void UpdateState(MafiaPlayerState player)
        {
            if (_text != null)
            {
                string status = player.IsAlive ? "" : "\n[МЁРТВ]";
                _text.text = $"#{player.ChairIndex + 1} {player.Name}{status}";
                _text.color = player.IsAlive ? Color.white : new Color(0.6f, 0.6f, 0.6f, 0.7f);
            }

            if (_bgImage != null)
            {
                if (!player.IsAlive)
                {
                    _bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
                }
                else if (player.IsLocal)
                {
                    _bgImage.color = new Color(0.15f, 0.45f, 0.75f, 0.85f);
                }
                else
                {
                    _bgImage.color = new Color(0.2f, 0.2f, 0.25f, 0.75f);
                }
            }
        }
    }
}
