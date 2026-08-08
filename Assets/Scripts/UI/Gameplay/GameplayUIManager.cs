using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mafia;
using UnityEngine.SceneManagement;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Instance { get; private set; }

    [Header("Canvas & Root Setup")]
    [SerializeField] private Canvas _mainCanvas;

    // UI Panels & Elements
    private GameObject _topBarPanel;
    private TextMeshProUGUI _phaseText;
    private TextMeshProUGUI _timerText;
    private Transform _playerBadgesContainer;
    private List<PlayerBadgeItem> _playerBadges = new List<PlayerBadgeItem>();

    private GameObject _roleRevealModal;
    private TextMeshProUGUI _roleRevealTitleText;
    private TextMeshProUGUI _roleRevealDescText;

    private GameObject _announcementBanner;
    private TextMeshProUGUI _announcementText;
    private Coroutine _announcementCoroutine;

    private GameObject _cardHandPanel;
    private List<CardButtonUI> _cardButtons = new List<CardButtonUI>();
    private TextMeshProUGUI _cardCooldownText;
    private TextMeshProUGUI _cardsRemainingText;

    private GameObject _actionTargetPanel;
    private TextMeshProUGUI _actionPromptText;
    private Transform _targetButtonsContainer;
    private List<TargetButtonUI> _targetButtons = new List<TargetButtonUI>();
    private Button _confirmActionButton;

    private GameObject _gameOverModal;
    private TextMeshProUGUI _gameOverTitleText;
    private Transform _gameOverRolesContainer;
    private Button _exitToMenuButton;

    private int _selectedTargetChair = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EnsureCanvasSetup();
        BindOrBuildUI();
    }

    private void Start()
    {
        if (MafiaGameManager.Instance != null)
        {
            SubscribeEvents();
        }
        else
        {
            StartCoroutine(WaitForGameManagerAndSubscribe());
        }
    }

    private IEnumerator WaitForGameManagerAndSubscribe()
    {
        while (MafiaGameManager.Instance == null) yield return null;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        MafiaGameManager.Instance.OnPhaseChanged += HandlePhaseChanged;
        MafiaGameManager.Instance.OnTimerUpdated += HandleTimerUpdated;
        MafiaGameManager.Instance.OnCardPlayed += HandleCardPlayed;
        MafiaGameManager.Instance.OnPlayerEliminated += HandlePlayerEliminated;
        MafiaGameManager.Instance.OnAnnouncement += ShowAnnouncement;
        MafiaGameManager.Instance.OnGameOver += HandleGameOver;

        SetupPlayerBadges();
    }

    private void OnDestroy()
    {
        if (MafiaGameManager.Instance != null)
        {
            MafiaGameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
            MafiaGameManager.Instance.OnTimerUpdated -= HandleTimerUpdated;
            MafiaGameManager.Instance.OnCardPlayed -= HandleCardPlayed;
            MafiaGameManager.Instance.OnPlayerEliminated -= HandlePlayerEliminated;
            MafiaGameManager.Instance.OnAnnouncement -= ShowAnnouncement;
            MafiaGameManager.Instance.OnGameOver -= HandleGameOver;
        }
    }

    private void Update()
    {
        // 3D chair / character raycast for targeting during voting or night action
        bool clicked = false;
        Vector2 screenPos = Vector2.zero;

        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicked = true;
            screenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        }
        else if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            clicked = true;
            screenPos = UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (clicked && Camera.main != null)
        {
            if (MafiaGameManager.Instance != null &&
               (MafiaGameManager.Instance.CurrentPhase == EGamePhase.DayVoting ||
                MafiaGameManager.Instance.CurrentPhase == EGamePhase.NightPhase))
            {
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    Chair chair = hit.collider.GetComponentInParent<Chair>();
                    if (chair != null)
                    {
                        var player = MafiaGameManager.Instance.Players.FirstOrDefault(p => p.ChairIndex == GetChairIndex(chair));
                        if (player != null && player.IsAlive && (!player.IsLocal || MafiaGameManager.Instance.CurrentPhase == EGamePhase.NightPhase))
                        {
                            SelectTarget(player.ChairIndex);
                        }
                    }
                }
            }
        }

        // Update card cooldown & card status in UI
        if (_cardHandPanel != null && _cardHandPanel.activeSelf && MafiaGameManager.Instance != null)
        {
            float cd = MafiaGameManager.Instance.CardCooldownRemaining;
            if (cd > 0)
            {
                if (_cardCooldownText != null) _cardCooldownText.text = $"Перезарядка: {cd:0.0}с";
            }
            else
            {
                if (_cardCooldownText != null) _cardCooldownText.text = "Готово к ходу!";
            }

            if (_cardsRemainingText != null)
            {
                _cardsRemainingText.text = $"Карты: {MafiaGameManager.Instance.CardsPlayedThisRound}/{MafiaGameManager.MaxCardsPerRound}";
            }

            UpdateCardButtonsState();
        }
    }

    private int GetChairIndex(Chair chair)
    {
        TableController tc = FindAnyObjectByType<TableController>();
        if (tc != null && tc.Table != null && tc.Table.Chairs != null)
        {
            for (int i = 0; i < tc.Table.Chairs.Length; i++)
            {
                if (tc.Table.Chairs[i] == chair) return i;
            }
        }
        return -1;
    }

    #region Canvas Setup & Binding
    private void EnsureCanvasSetup()
    {
        if (_mainCanvas == null)
        {
            GameObject canvasGo = GameObject.Find("GameplayCanvas");
            if (canvasGo != null) _mainCanvas = canvasGo.GetComponent<Canvas>();
        }

        if (_mainCanvas == null) _mainCanvas = FindAnyObjectByType<Canvas>();

        if (_mainCanvas == null)
        {
            GameObject canvasGo = new GameObject("GameplayCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _mainCanvas = canvasGo.GetComponent<Canvas>();
            _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
    }

    private void BindOrBuildUI()
    {
        Transform parent = _mainCanvas.transform;

        // TopBar
        Transform topBarTransform = parent.Find("TopBarPanel");
        if (topBarTransform != null)
        {
            _topBarPanel = topBarTransform.gameObject;
            Transform pTxt = topBarTransform.Find("PhaseText");
            if (pTxt != null) _phaseText = pTxt.GetComponent<TextMeshProUGUI>();

            Transform tTxt = topBarTransform.Find("TimerText");
            if (tTxt != null) _timerText = tTxt.GetComponent<TextMeshProUGUI>();

            Transform bContainer = topBarTransform.Find("BadgesContainer");
            if (bContainer != null) _playerBadgesContainer = bContainer;
        }
        else
        {
            BuildTopBar(parent);
        }

        // Announcement
        Transform annTransform = parent.Find("AnnouncementBanner");
        if (annTransform != null)
        {
            _announcementBanner = annTransform.gameObject;
            Transform aTxt = annTransform.Find("Text");
            if (aTxt != null) _announcementText = aTxt.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            BuildAnnouncementBanner(parent);
        }

        // Card Hand Panel
        Transform cardTransform = parent.Find("CardHandPanel");
        if (cardTransform != null)
        {
            _cardHandPanel = cardTransform.gameObject;
            Transform cdTxt = cardTransform.Find("CooldownText");
            if (cdTxt != null) _cardCooldownText = cdTxt.GetComponent<TextMeshProUGUI>();

            Transform rTxt = cardTransform.Find("RemText");
            if (rTxt != null) _cardsRemainingText = rTxt.GetComponent<TextMeshProUGUI>();

            Transform container = cardTransform.Find("CardsContainer");
            if (container != null)
            {
                _cardButtons.Clear();
                int idx = 0;
                foreach (Transform child in container)
                {
                    int cardIndex = idx;
                    Button btn = child.GetComponent<Button>();
                    Image bg = child.GetComponent<Image>();
                    TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();

                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => OnCardClicked(cardIndex));
                    }

                    _cardButtons.Add(new CardButtonUI { Root = child.gameObject, Button = btn, Text = tmp, Background = bg });
                    idx++;
                }
            }
        }
        else
        {
            BuildCardHandPanel(parent);
        }

        // Modals & Panels if missing
        if (parent.Find("RoleRevealModal") != null) _roleRevealModal = parent.Find("RoleRevealModal").gameObject;
        else BuildRoleRevealModal(parent);

        if (parent.Find("ActionTargetPanel") != null) _actionTargetPanel = parent.Find("ActionTargetPanel").gameObject;
        else BuildActionTargetPanel(parent);

        if (parent.Find("GameOverModal") != null) _gameOverModal = parent.Find("GameOverModal").gameObject;
        else BuildGameOverModal(parent);
    }
    #endregion

    #region Dynamic UI Generation
    private void BuildTopBar(Transform parent)
    {
        _topBarPanel = CreateUIObject("TopBarPanel", parent);
        SetRect(_topBarPanel, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -10), new Vector2(0, 85));

        Image bg = _topBarPanel.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.12f, 0.85f);

        GameObject phaseObj = CreateUIObject("PhaseText", _topBarPanel.transform);
        SetRect(phaseObj, new Vector2(0.35f, 0.5f), new Vector2(0.65f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        _phaseText = phaseObj.AddComponent<TextMeshProUGUI>();
        _phaseText.text = "ЗАГРУЗКА...";
        _phaseText.fontSize = 24;
        _phaseText.fontStyle = FontStyles.Bold;
        _phaseText.alignment = TextAlignmentOptions.Center;
        _phaseText.color = new Color(1f, 0.84f, 0f, 1f);

        GameObject timerObj = CreateUIObject("TimerText", _topBarPanel.transform);
        SetRect(timerObj, new Vector2(0.35f, 0.05f), new Vector2(0.65f, 0.45f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        _timerText = timerObj.AddComponent<TextMeshProUGUI>();
        _timerText.text = "00";
        _timerText.fontSize = 20;
        _timerText.alignment = TextAlignmentOptions.Center;
        _timerText.color = Color.white;

        GameObject badgesObj = CreateUIObject("BadgesContainer", _topBarPanel.transform);
        SetRect(badgesObj, new Vector2(0.02f, 0.05f), new Vector2(0.98f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        HorizontalLayoutGroup hlg = badgesObj.AddComponent<HorizontalLayoutGroup>();
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.spacing = 12f;
        _playerBadgesContainer = badgesObj.transform;
    }

    private void BuildRoleRevealModal(Transform parent)
    {
        _roleRevealModal = CreateUIObject("RoleRevealModal", parent);
        SetRect(_roleRevealModal, new Vector2(0.25f, 0.25f), new Vector2(0.75f, 0.75f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        Image bg = _roleRevealModal.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.1f, 0.95f);

        GameObject titleObj = CreateUIObject("Title", _roleRevealModal.transform);
        SetRect(titleObj, new Vector2(0.05f, 0.7f), new Vector2(0.95f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        _roleRevealTitleText = titleObj.AddComponent<TextMeshProUGUI>();
        _roleRevealTitleText.fontSize = 32;
        _roleRevealTitleText.fontStyle = FontStyles.Bold;
        _roleRevealTitleText.alignment = TextAlignmentOptions.Center;

        GameObject descObj = CreateUIObject("Desc", _roleRevealModal.transform);
        SetRect(descObj, new Vector2(0.05f, 0.25f), new Vector2(0.95f, 0.65f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        _roleRevealDescText = descObj.AddComponent<TextMeshProUGUI>();
        _roleRevealDescText.fontSize = 20;
        _roleRevealDescText.alignment = TextAlignmentOptions.Center;
        _roleRevealDescText.color = Color.white;

        CreateButton("CloseButton", _roleRevealModal.transform, "ПОНЯТНО", new Vector2(0.35f, 0.05f), new Vector2(0.65f, 0.2f), () =>
        {
            _roleRevealModal.SetActive(false);
        });

        _roleRevealModal.SetActive(false);
    }

    private void BuildAnnouncementBanner(Transform parent)
    {
        _announcementBanner = CreateUIObject("AnnouncementBanner", parent);
        SetRect(_announcementBanner, new Vector2(0.2f, 0.72f), new Vector2(0.8f, 0.82f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        Image bg = _announcementBanner.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.85f);

        GameObject txtObj = CreateUIObject("Text", _announcementBanner.transform);
        SetRect(txtObj, new Vector2(0.02f, 0.05f), new Vector2(0.98f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        _announcementText = txtObj.AddComponent<TextMeshProUGUI>();
        _announcementText.fontSize = 22;
        _announcementText.fontStyle = FontStyles.Bold;
        _announcementText.alignment = TextAlignmentOptions.Center;
        _announcementText.color = Color.yellow;

        _announcementBanner.SetActive(false);
    }

    private void BuildCardHandPanel(Transform parent)
    {
        _cardHandPanel = CreateUIObject("CardHandPanel", parent);
        SetRect(_cardHandPanel, new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.22f), new Vector2(0.5f, 0f), Vector2.zero, Vector2.zero);

        Image bg = _cardHandPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);

        GameObject cdObj = CreateUIObject("CooldownText", _cardHandPanel.transform);
        SetRect(cdObj, new Vector2(0.02f, 0.82f), new Vector2(0.48f, 0.98f), new Vector2(0f, 1f), Vector2.zero, Vector2.zero);
        _cardCooldownText = cdObj.AddComponent<TextMeshProUGUI>();
        _cardCooldownText.fontSize = 15;
        _cardCooldownText.color = Color.cyan;

        GameObject remObj = CreateUIObject("RemText", _cardHandPanel.transform);
        SetRect(remObj, new Vector2(0.52f, 0.82f), new Vector2(0.98f, 0.98f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        _cardsRemainingText = remObj.AddComponent<TextMeshProUGUI>();
        _cardsRemainingText.fontSize = 15;
        _cardsRemainingText.alignment = TextAlignmentOptions.Right;
        _cardsRemainingText.color = Color.white;

        GameObject container = CreateUIObject("CardsContainer", _cardHandPanel.transform);
        SetRect(container, new Vector2(0.01f, 0.05f), new Vector2(0.99f, 0.8f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        HorizontalLayoutGroup hlg = container.AddComponent<HorizontalLayoutGroup>();
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.spacing = 10f;

        _cardButtons.Clear();
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            GameObject cardGo = CreateUIObject($"Card_{i}", container.transform);
            Image cardBg = cardGo.AddComponent<Image>();
            cardBg.color = new Color(0.2f, 0.35f, 0.5f, 0.95f);

            Button btn = cardGo.AddComponent<Button>();

            GameObject textGo = CreateUIObject("Text", cardGo.transform);
            SetRect(textGo, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 15;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            btn.onClick.AddListener(() => OnCardClicked(index));

            _cardButtons.Add(new CardButtonUI { Root = cardGo, Button = btn, Text = tmp, Background = cardBg });
        }

        _cardHandPanel.SetActive(false);
    }

    private void BuildActionTargetPanel(Transform parent)
    {
        _actionTargetPanel = CreateUIObject("ActionTargetPanel", parent);
        SetRect(_actionTargetPanel, new Vector2(0.2f, 0.02f), new Vector2(0.8f, 0.22f), new Vector2(0.5f, 0f), Vector2.zero, Vector2.zero);

        Image bg = _actionTargetPanel.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.08f, 0.08f, 0.9f);

        GameObject promptObj = CreateUIObject("PromptText", _actionTargetPanel.transform);
        SetRect(promptObj, new Vector2(0.02f, 0.75f), new Vector2(0.98f, 0.98f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero);
        _actionPromptText = promptObj.AddComponent<TextMeshProUGUI>();
        _actionPromptText.fontSize = 20;
        _actionPromptText.fontStyle = FontStyles.Bold;
        _actionPromptText.alignment = TextAlignmentOptions.Center;
        _actionPromptText.color = Color.yellow;

        GameObject targetsObj = CreateUIObject("TargetsContainer", _actionTargetPanel.transform);
        SetRect(targetsObj, new Vector2(0.02f, 0.25f), new Vector2(0.98f, 0.7f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        HorizontalLayoutGroup hlg = targetsObj.AddComponent<HorizontalLayoutGroup>();
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.spacing = 8f;
        _targetButtonsContainer = targetsObj.transform;

        GameObject confirmObj = CreateButton("ConfirmButton", _actionTargetPanel.transform, "ПОДТВЕРДИТЬ ВЫБОР", new Vector2(0.35f, 0.02f), new Vector2(0.65f, 0.22f), ConfirmTargetSelection);
        _confirmActionButton = confirmObj.GetComponent<Button>();

        _actionTargetPanel.SetActive(false);
    }

    private void BuildGameOverModal(Transform parent)
    {
        _gameOverModal = CreateUIObject("GameOverModal", parent);
        SetRect(_gameOverModal, new Vector2(0.15f, 0.15f), new Vector2(0.85f, 0.85f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        Image bg = _gameOverModal.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.08f, 0.98f);

        GameObject titleObj = CreateUIObject("Title", _gameOverModal.transform);
        SetRect(titleObj, new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.98f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero);
        _gameOverTitleText = titleObj.AddComponent<TextMeshProUGUI>();
        _gameOverTitleText.fontSize = 36;
        _gameOverTitleText.fontStyle = FontStyles.Bold;
        _gameOverTitleText.alignment = TextAlignmentOptions.Center;

        GameObject container = CreateUIObject("RolesContainer", _gameOverModal.transform);
        SetRect(container, new Vector2(0.05f, 0.2f), new Vector2(0.95f, 0.8f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        VerticalLayoutGroup vlg = container.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.spacing = 6f;
        _gameOverRolesContainer = container.transform;

        GameObject exitBtn = CreateButton("ExitButton", _gameOverModal.transform, "В ГЛАВНОЕ МЕНЮ", new Vector2(0.35f, 0.04f), new Vector2(0.65f, 0.16f), () =>
        {
            SceneManager.LoadScene("Loading");
        });
        _exitToMenuButton = exitBtn.GetComponent<Button>();

        _gameOverModal.SetActive(false);
    }
    #endregion

    #region Event Handlers
    private void HandlePhaseChanged(EGamePhase phase, int round)
    {
        if (_phaseText != null)
        {
            switch (phase)
            {
                case EGamePhase.RoleReveal:
                    _phaseText.text = "ЗНАКОМСТВО С РОЛЬЮ";
                    ShowRoleReveal();
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

        if (_cardHandPanel != null) _cardHandPanel.SetActive(phase == EGamePhase.DayDiscussion && LocalPlayerAlive());
        if (_actionTargetPanel != null) _actionTargetPanel.SetActive((phase == EGamePhase.DayVoting || phase == EGamePhase.NightPhase) && LocalPlayerAlive());

        if (phase == EGamePhase.DayDiscussion)
        {
            UpdateCardButtonsState();
        }
        else if (phase == EGamePhase.DayVoting || phase == EGamePhase.NightPhase)
        {
            SetupTargetButtons(phase);
        }

        UpdatePlayerBadgesState();
    }

    private bool LocalPlayerAlive()
    {
        return MafiaGameManager.Instance != null &&
               MafiaGameManager.Instance.LocalPlayer != null &&
               MafiaGameManager.Instance.LocalPlayer.IsAlive;
    }

    private void HandleTimerUpdated(float remaining)
    {
        if (_timerText != null)
        {
            int secs = Mathf.Max(0, Mathf.CeilToInt(remaining));
            _timerText.text = $"{secs:00}";
        }
    }

    private void HandleCardPlayed(MafiaPlayerState player, CardData card)
    {
        if (player.IsLocal)
        {
            UpdateCardButtonsState();
        }
    }

    private void HandlePlayerEliminated(MafiaPlayerState player, string message)
    {
        ShowAnnouncement(message);
        UpdatePlayerBadgesState();
    }

    private void HandleGameOver(EPlayerRole winningFaction)
    {
        if (_gameOverModal == null) return;

        _gameOverModal.SetActive(true);
        if (_gameOverTitleText != null)
        {
            if (winningFaction == EPlayerRole.Civilian)
            {
                _gameOverTitleText.text = "ПОБЕДА МИРНЫХ ЖИТЕЛЕЙ!";
                _gameOverTitleText.color = Color.green;
            }
            else
            {
                _gameOverTitleText.text = "ПОБЕДА МАФИИ!";
                _gameOverTitleText.color = Color.red;
            }
        }

        foreach (Transform child in _gameOverRolesContainer) Destroy(child.gameObject);

        if (MafiaGameManager.Instance != null)
        {
            foreach (var p in MafiaGameManager.Instance.Players)
            {
                GameObject item = CreateUIObject($"RoleItem_{p.ChairIndex}", _gameOverRolesContainer);
                Image bg = item.AddComponent<Image>();
                bg.color = p.IsLocal ? new Color(0.2f, 0.4f, 0.6f, 0.8f) : new Color(0.15f, 0.15f, 0.2f, 0.8f);

                GameObject textGo = CreateUIObject("Text", item.transform);
                SetRect(textGo, new Vector2(0.02f, 0.1f), new Vector2(0.98f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
                TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
                tmp.text = $"#{p.ChairIndex + 1} {p.Name} — <color=yellow>{p.GetRoleName()}</color> ({p.GetTeamName()})";
                tmp.fontSize = 18;
                tmp.alignment = TextAlignmentOptions.Left;
                tmp.color = Color.white;
            }
        }
    }

    public void ShowAnnouncement(string text)
    {
        if (_announcementCoroutine != null) StopCoroutine(_announcementCoroutine);
        _announcementCoroutine = StartCoroutine(AnnouncementRoutine(text));
    }

    private IEnumerator AnnouncementRoutine(string text)
    {
        if (_announcementBanner != null) _announcementBanner.SetActive(true);
        if (_announcementText != null) _announcementText.text = text;

        yield return new WaitForSeconds(3.5f);

        if (_announcementBanner != null) _announcementBanner.SetActive(false);
    }

    private void ShowRoleReveal()
    {
        if (_roleRevealModal == null || MafiaGameManager.Instance == null || MafiaGameManager.Instance.LocalPlayer == null) return;

        var local = MafiaGameManager.Instance.LocalPlayer;
        _roleRevealModal.SetActive(true);

        if (_roleRevealTitleText != null)
        {
            _roleRevealTitleText.text = $"ВАША РОЛЬ: {local.GetRoleName().ToUpper()}";
            _roleRevealTitleText.color = local.Role == EPlayerRole.Mafia ? Color.red : Color.cyan;
        }

        if (_roleRevealDescText != null)
        {
            switch (local.Role)
            {
                case EPlayerRole.Mafia:
                    _roleRevealDescText.text = "Вы — член Мафии.\nКаждую ночь вы и ваши сообщники выбираете жертву.\nЦель: Исключить всех мирных жителей.";
                    break;
                case EPlayerRole.Doctor:
                    _roleRevealDescText.text = "Вы — Доктор.\nКаждую ночь вы можете спасти одного игрока от нападения мафии.\nЦель: Найти и выгнать всех членов мафии.";
                    break;
                case EPlayerRole.Commissioner:
                    _roleRevealDescText.text = "Вы — Комиссар (Шериф).\nКаждую ночь вы проверяете одного игрока и узнаёте, мафия он или нет.\nЦель: Возглавить расследование и вычислить мафию.";
                    break;
                case EPlayerRole.Civilian:
                    _roleRevealDescText.text = "Вы — Мирный Житель.\nДнём участвуйте в обсуждениях, наблюдайте за поведением и голосуйте.\nЦель: Вычислить и исключить всю мафию.";
                    break;
            }
        }
    }
    #endregion

    #region Cards & Actions Logic
    private void UpdateCardButtonsState()
    {
        if (MafiaGameManager.Instance == null || _cardButtons.Count == 0) return;

        var hand = MafiaGameManager.Instance.LocalHand;
        bool canPlay = MafiaGameManager.Instance.CanPlayCard();

        for (int i = 0; i < _cardButtons.Count; i++)
        {
            var btnUI = _cardButtons[i];
            if (i < hand.Count)
            {
                btnUI.Root.SetActive(true);
                CardData card = hand[i];
                btnUI.Text.text = card.Text;
                btnUI.Background.color = canPlay ? card.GetColor() : new Color(0.3f, 0.3f, 0.3f, 0.6f);
                btnUI.Button.interactable = canPlay;
            }
            else
            {
                btnUI.Root.SetActive(false);
            }
        }
    }

    private void OnCardClicked(int index)
    {
        if (MafiaGameManager.Instance != null && MafiaGameManager.Instance.CanPlayCard())
        {
            MafiaGameManager.Instance.PlayLocalCard(index);
        }
    }

    private void SetupTargetButtons(EGamePhase phase)
    {
        if (_targetButtonsContainer == null) return;

        foreach (Transform child in _targetButtonsContainer) Destroy(child.gameObject);
        _targetButtons.Clear();
        _selectedTargetChair = -1;

        if (MafiaGameManager.Instance == null || MafiaGameManager.Instance.LocalPlayer == null) return;

        var local = MafiaGameManager.Instance.LocalPlayer;

        if (phase == EGamePhase.DayVoting)
        {
            if (_actionPromptText != null) _actionPromptText.text = "ГОЛОСОВАНИЕ: Выберите подозреваемого";
        }
        else if (phase == EGamePhase.NightPhase)
        {
            switch (local.Role)
            {
                case EPlayerRole.Mafia:
                    if (_actionPromptText != null) _actionPromptText.text = "НОЧЬ: Выберите цель для ликвидации";
                    break;
                case EPlayerRole.Doctor:
                    if (_actionPromptText != null) _actionPromptText.text = "НОЧЬ: Выберите кого вылечить";
                    break;
                case EPlayerRole.Commissioner:
                    if (_actionPromptText != null) _actionPromptText.text = "НОЧЬ: Выберите кого проверить";
                    break;
                case EPlayerRole.Civilian:
                    if (_actionPromptText != null) _actionPromptText.text = "НОЧЬ: Город засыпает... Вы отдыхаете.";
                    break;
            }
        }

        var living = MafiaGameManager.Instance.Players.Where(p => p.IsAlive).ToList();
        foreach (var p in living)
        {
            if (phase == EGamePhase.DayVoting && p.IsLocal) continue;

            int targetChair = p.ChairIndex;
            GameObject btnGo = CreateUIObject($"Target_{targetChair}", _targetButtonsContainer);
            Image bg = btnGo.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.25f, 0.35f, 0.9f);

            Button btn = btnGo.AddComponent<Button>();

            GameObject txtGo = CreateUIObject("Text", btnGo.transform);
            SetRect(txtGo, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            TextMeshProUGUI tmp = txtGo.AddComponent<TextMeshProUGUI>();
            tmp.text = $"#{p.ChairIndex + 1}\n{p.Name}";
            tmp.fontSize = 14;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            btn.onClick.AddListener(() => SelectTarget(targetChair));

            _targetButtons.Add(new TargetButtonUI { ChairIndex = targetChair, Background = bg, Text = tmp });
        }
    }

    private void SelectTarget(int chairIndex)
    {
        _selectedTargetChair = chairIndex;

        foreach (var tb in _targetButtons)
        {
            if (tb.ChairIndex == chairIndex)
            {
                tb.Background.color = Color.red;
            }
            else
            {
                tb.Background.color = new Color(0.2f, 0.25f, 0.35f, 0.9f);
            }
        }
    }

    private void ConfirmTargetSelection()
    {
        if (_selectedTargetChair < 0 || MafiaGameManager.Instance == null) return;

        if (MafiaGameManager.Instance.CurrentPhase == EGamePhase.DayVoting)
        {
            MafiaGameManager.Instance.CastLocalVote(_selectedTargetChair);
            ShowAnnouncement($"Вы проголосовали за игрока #{_selectedTargetChair + 1}");
        }
        else if (MafiaGameManager.Instance.CurrentPhase == EGamePhase.NightPhase)
        {
            MafiaGameManager.Instance.PerformNightAction(_selectedTargetChair);
            ShowAnnouncement($"Выбор сделан!");
        }

        if (_actionTargetPanel != null) _actionTargetPanel.SetActive(false);
    }
    #endregion

    #region Badges Setup
    private void SetupPlayerBadges()
    {
        if (_playerBadgesContainer == null || MafiaGameManager.Instance == null) return;

        foreach (var b in _playerBadges)
        {
            if (b.Root != null) Destroy(b.Root);
        }
        _playerBadges.Clear();

        foreach (var p in MafiaGameManager.Instance.Players)
        {
            GameObject badgeGo = CreateUIObject($"Badge_{p.ChairIndex}", _playerBadgesContainer);
            Image bg = badgeGo.AddComponent<Image>();

            GameObject txtObj = CreateUIObject("Text", badgeGo.transform);
            SetRect(txtObj, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Center;

            PlayerBadgeItem item = new PlayerBadgeItem { ChairIndex = p.ChairIndex, Root = badgeGo, Background = bg, Text = tmp };
            _playerBadges.Add(item);
        }

        UpdatePlayerBadgesState();
    }

    private void UpdatePlayerBadgesState()
    {
        if (MafiaGameManager.Instance == null) return;

        foreach (var p in MafiaGameManager.Instance.Players)
        {
            var badge = _playerBadges.FirstOrDefault(b => b.ChairIndex == p.ChairIndex);
            if (badge != null)
            {
                string status = p.IsAlive ? "" : "\n<color=red>[МЁРТВ]</color>";
                badge.Text.text = $"#{p.ChairIndex + 1} {p.Name}{status}";

                if (!p.IsAlive)
                {
                    badge.Background.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
                    badge.Text.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
                }
                else if (p.IsLocal)
                {
                    badge.Background.color = new Color(0.15f, 0.45f, 0.85f, 0.9f);
                    badge.Text.color = Color.white;
                }
                else
                {
                    badge.Background.color = new Color(0.2f, 0.22f, 0.28f, 0.8f);
                    badge.Text.color = Color.white;
                }
            }
        }
    }
    #endregion

    #region Helper UI Builders
    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = CreateUIObject(name, parent);
        SetRect(btnObj, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        Image bg = btnObj.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.6f, 0.3f, 0.9f);

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        GameObject txtObj = CreateUIObject("Text", btnObj.transform);
        SetRect(txtObj, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 18;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return btnObj;
    }

    private void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    private class CardButtonUI
    {
        public GameObject Root;
        public Button Button;
        public TextMeshProUGUI Text;
        public Image Background;
    }

    private class TargetButtonUI
    {
        public int ChairIndex;
        public Image Background;
        public TextMeshProUGUI Text;
    }

    private class PlayerBadgeItem
    {
        public int ChairIndex;
        public GameObject Root;
        public Image Background;
        public TextMeshProUGUI Text;
    }
    #endregion
}
