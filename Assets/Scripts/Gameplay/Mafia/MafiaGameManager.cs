using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mafia
{
    public class MafiaGameManager : MonoBehaviour
    {
        public static MafiaGameManager Instance { get; private set; }

        public EGamePhase CurrentPhase { get; private set; } = EGamePhase.None;
        public int CurrentRound { get; private set; } = 1;
        public float PhaseTimeRemaining { get; private set; } = 0f;

        public List<MafiaPlayerState> Players { get; private set; } = new List<MafiaPlayerState>();
        public MafiaPlayerState LocalPlayer => Players.FirstOrDefault(p => p.IsLocal);

        public List<CardData> LocalHand { get; private set; } = new List<CardData>();

        // Phase Duration settings
        public float RoleRevealDuration = 4f;
        public float DayDiscussionDuration = 40f;
        public float DayVotingDuration = 20f;
        public float NightPhaseDuration = 20f;

        // Selection variables
        public int LocalVoteTargetChair { get; private set; } = -1;
        public int LocalNightTargetChair { get; private set; } = -1;

        // Card limits per discussion round
        public int CardsPlayedThisRound { get; private set; } = 0;
        public float CardCooldownRemaining { get; private set; } = 0f;
        public const float CardCooldownDuration = 5f;
        public const int MaxCardsPerRound = 5;

        // Events
        public event Action<EGamePhase, int> OnPhaseChanged;
        public event Action<float> OnTimerUpdated;
        public event Action<MafiaPlayerState, CardData> OnCardPlayed;
        public event Action<MafiaPlayerState, MafiaPlayerState> OnPlayerVoted;
        public event Action<MafiaPlayerState, string> OnPlayerEliminated;
        public event Action<string> OnAnnouncement;
        public event Action<EPlayerRole> OnGameOver;

        private TableController _tableController;
        private Coroutine _phaseCoroutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (CardCooldownRemaining > 0)
            {
                CardCooldownRemaining -= Time.deltaTime;
            }
        }

        public void InitializeGame(TableController tableController, List<Michitai.Multiplayer.Rooms.RoomPlayer<PlayerData>> roomPlayers, PlayerData localPlayerData)
        {
            _tableController = tableController;
            Players.Clear();

            // Prepare 6 players roles
            List<EPlayerRole> roles = new List<EPlayerRole>
            {
                EPlayerRole.Mafia,
                EPlayerRole.Mafia,
                EPlayerRole.Doctor,
                EPlayerRole.Commissioner,
                EPlayerRole.Civilian,
                EPlayerRole.Civilian
            };

            // Shuffle roles
            roles = roles.OrderBy(r => UnityEngine.Random.value).ToList();

            for (int i = 0; i < 6; i++)
            {
                var rPlayer = (roomPlayers != null && i < roomPlayers.Count) ? roomPlayers[i] : null;
                PlayerData pData = rPlayer?.PlayerData;
                bool isLocal = rPlayer != null ? rPlayer.is_local : (i == 0);
                string pId = rPlayer != null ? rPlayer.player_id.ToString() : $"bot_{i + 1}";

                if (pData == null)
                {
                    pData = new PlayerData();
                    pData.character = new CharacterCustomData();
                }

                bool isBot = !isLocal;
                string displayName = isLocal ? "Игрок (Вы)" : (rPlayer != null && !string.IsNullOrEmpty(rPlayer.player_name) ? rPlayer.player_name : $"Игрок {i + 1}");

                MafiaPlayerState playerState = new MafiaPlayerState(
                    chairIndex: i,
                    playerId: pId,
                    name: displayName,
                    playerData: pData,
                    role: roles[i],
                    isLocal: isLocal,
                    isBot: isBot
                );

                Players.Add(playerState);
            }

            LocalHand = CardDeck.GetInitialHand(4);

            StartGame();
        }

        public void StartGame()
        {
            CurrentRound = 1;
            SetPhase(EGamePhase.RoleReveal);
        }

        private void SetPhase(EGamePhase newPhase)
        {
            CurrentPhase = newPhase;
            if (_phaseCoroutine != null) StopCoroutine(_phaseCoroutine);

            OnPhaseChanged?.Invoke(CurrentPhase, CurrentRound);

            switch (CurrentPhase)
            {
                case EGamePhase.RoleReveal:
                    _phaseCoroutine = StartCoroutine(RoleRevealRoutine());
                    break;
                case EGamePhase.DayDiscussion:
                    _phaseCoroutine = StartCoroutine(DayDiscussionRoutine());
                    break;
                case EGamePhase.DayVoting:
                    _phaseCoroutine = StartCoroutine(DayVotingRoutine());
                    break;
                case EGamePhase.NightPhase:
                    _phaseCoroutine = StartCoroutine(NightPhaseRoutine());
                    break;
                case EGamePhase.GameOver:
                    break;
            }
        }

        #region Role Reveal Phase
        private IEnumerator RoleRevealRoutine()
        {
            PhaseTimeRemaining = RoleRevealDuration;
            while (PhaseTimeRemaining > 0)
            {
                PhaseTimeRemaining -= Time.deltaTime;
                OnTimerUpdated?.Invoke(PhaseTimeRemaining);
                yield return null;
            }

            SetPhase(EGamePhase.DayDiscussion);
        }
        #endregion

        #region Day Discussion Phase
        private IEnumerator DayDiscussionRoutine()
        {
            CardsPlayedThisRound = 0;
            CardCooldownRemaining = 0;
            if (NightLightingController.Instance != null)
            {
                NightLightingController.Instance.SetNight(false);
            }

            OnAnnouncement?.Invoke($"ДЕНЬ {CurrentRound}\nНачалось дневное обсуждение!");

            PhaseTimeRemaining = DayDiscussionDuration;
            float botCardTimer = UnityEngine.Random.Range(3f, 7f);

            while (PhaseTimeRemaining > 0)
            {
                PhaseTimeRemaining -= Time.deltaTime;
                botCardTimer -= Time.deltaTime;

                if (botCardTimer <= 0)
                {
                    BotPlayCard();
                    botCardTimer = UnityEngine.Random.Range(5f, 10f);
                }

                OnTimerUpdated?.Invoke(PhaseTimeRemaining);
                yield return null;
            }

            SetPhase(EGamePhase.DayVoting);
        }

        public bool CanPlayCard()
        {
            return CurrentPhase == EGamePhase.DayDiscussion &&
                   LocalPlayer != null && LocalPlayer.IsAlive &&
                   CardsPlayedThisRound < MaxCardsPerRound &&
                   CardCooldownRemaining <= 0;
        }

        public void PlayLocalCard(int handIndex)
        {
            if (!CanPlayCard() || handIndex < 0 || handIndex >= LocalHand.Count) return;

            CardData playedCard = LocalHand[handIndex];
            CardsPlayedThisRound++;
            CardCooldownRemaining = CardCooldownDuration;

            // Replace card in hand
            LocalHand[handIndex] = CardDeck.GetRandomCard();

            // Trigger speech bubble
            TriggerCardPlayed(LocalPlayer, playedCard);
        }

        private void BotPlayCard()
        {
            var livingBots = Players.Where(p => p.IsBot && p.IsAlive).ToList();
            if (livingBots.Count == 0) return;

            MafiaPlayerState bot = livingBots[UnityEngine.Random.Range(0, livingBots.Count)];
            CardData card = CardDeck.GetRandomCard();

            TriggerCardPlayed(bot, card);
        }

        private void TriggerCardPlayed(MafiaPlayerState player, CardData card)
        {
            OnCardPlayed?.Invoke(player, card);

            if (_tableController != null && _tableController.Table != null)
            {
                Chair chair = _tableController.Table.GetChair(player.ChairIndex);
                if (chair != null && chair.Character != null)
                {
                    SpeechBubbleManager.Instance?.ShowSpeechBubble(
                        chair.Character.transform,
                        card.Text,
                        card.GetColor(),
                        player.Name
                    );
                }
            }
        }
        #endregion

        #region Day Voting Phase
        private IEnumerator DayVotingRoutine()
        {
            LocalVoteTargetChair = -1;
            foreach (var p in Players) p.VotesReceived = 0;

            OnAnnouncement?.Invoke($"ГОЛОСОВАНИЕ\nВыберите подозреваемого для исключения!");

            PhaseTimeRemaining = DayVotingDuration;

            while (PhaseTimeRemaining > 0)
            {
                PhaseTimeRemaining -= Time.deltaTime;
                OnTimerUpdated?.Invoke(PhaseTimeRemaining);
                yield return null;
            }

            // Process Votes
            ProcessVotes();
        }

        public void CastLocalVote(int targetChair)
        {
            if (CurrentPhase != EGamePhase.DayVoting || LocalPlayer == null || !LocalPlayer.IsAlive) return;

            LocalVoteTargetChair = targetChair;
            OnPlayerVoted?.Invoke(LocalPlayer, Players.FirstOrDefault(p => p.ChairIndex == targetChair));
        }

        private void ProcessVotes()
        {
            List<MafiaPlayerState> livingPlayers = Players.Where(p => p.IsAlive).ToList();

            foreach (var voter in livingPlayers)
            {
                int targetChair = -1;
                if (voter.IsLocal)
                {
                    targetChair = LocalVoteTargetChair;
                }
                else
                {
                    // Bot logic: vote for someone alive excluding self
                    var candidates = livingPlayers.Where(p => p.ChairIndex != voter.ChairIndex).ToList();
                    if (candidates.Count > 0)
                    {
                        targetChair = candidates[UnityEngine.Random.Range(0, candidates.Count)].ChairIndex;
                    }
                }

                if (targetChair >= 0)
                {
                    var target = Players.FirstOrDefault(p => p.ChairIndex == targetChair);
                    if (target != null) target.VotesReceived++;
                }
            }

            // Find highest vote count
            int maxVotes = livingPlayers.Max(p => p.VotesReceived);
            var topVoted = livingPlayers.Where(p => p.VotesReceived == maxVotes).ToList();

            if (maxVotes > 0 && topVoted.Count == 1)
            {
                MafiaPlayerState executed = topVoted[0];
                EliminatePlayer(executed, "был исключён по решению голосования!");
            }
            else
            {
                OnAnnouncement?.Invoke("Ничья! Никто не был исключён сегодня.");
            }

            if (CheckWinCondition()) return;

            SetPhase(EGamePhase.NightPhase);
        }
        #endregion

        #region Night Phase
        private IEnumerator NightPhaseRoutine()
        {
            LocalNightTargetChair = -1;

            if (NightLightingController.Instance != null)
            {
                NightLightingController.Instance.SetNight(true);
            }

            OnAnnouncement?.Invoke($"НОЧЬ {CurrentRound}\nГород засыпает. Мафия и особые роли делают свой выбор...");

            PhaseTimeRemaining = NightPhaseDuration;

            while (PhaseTimeRemaining > 0)
            {
                PhaseTimeRemaining -= Time.deltaTime;
                OnTimerUpdated?.Invoke(PhaseTimeRemaining);
                yield return null;
            }

            // Process Night Actions
            ProcessNightActions();
        }

        public void PerformNightAction(int targetChair)
        {
            if (CurrentPhase != EGamePhase.NightPhase || LocalPlayer == null || !LocalPlayer.IsAlive) return;

            LocalNightTargetChair = targetChair;

            // Commissioner instant check feedback
            if (LocalPlayer.Role == EPlayerRole.Commissioner && targetChair >= 0)
            {
                var target = Players.FirstOrDefault(p => p.ChairIndex == targetChair);
                if (target != null)
                {
                    string isMafiaText = target.Role == EPlayerRole.Mafia ? "МАФИЯ!" : "МИРНЫЙ";
                    OnAnnouncement?.Invoke($"Результат проверки комиссара:\n{target.Name} — {isMafiaText}");
                }
            }
        }

        private void ProcessNightActions()
        {
            List<MafiaPlayerState> livingPlayers = Players.Where(p => p.IsAlive).ToList();

            MafiaPlayerState mafiaTarget = null;
            MafiaPlayerState doctorTarget = null;

            // Mafia Action
            var mafiaPlayers = livingPlayers.Where(p => p.Role == EPlayerRole.Mafia).ToList();
            if (mafiaPlayers.Count > 0)
            {
                int targetChair = -1;
                var localMafia = mafiaPlayers.FirstOrDefault(p => p.IsLocal);

                if (localMafia != null && LocalNightTargetChair >= 0)
                {
                    targetChair = LocalNightTargetChair;
                }
                else
                {
                    var nonMafia = livingPlayers.Where(p => p.Role != EPlayerRole.Mafia).ToList();
                    if (nonMafia.Count > 0)
                    {
                        targetChair = nonMafia[UnityEngine.Random.Range(0, nonMafia.Count)].ChairIndex;
                    }
                }

                if (targetChair >= 0)
                {
                    mafiaTarget = Players.FirstOrDefault(p => p.ChairIndex == targetChair);
                }
            }

            // Doctor Action
            var doctor = livingPlayers.FirstOrDefault(p => p.Role == EPlayerRole.Doctor);
            if (doctor != null)
            {
                int targetChair = -1;
                if (doctor.IsLocal && LocalNightTargetChair >= 0)
                {
                    targetChair = LocalNightTargetChair;
                }
                else
                {
                    targetChair = livingPlayers[UnityEngine.Random.Range(0, livingPlayers.Count)].ChairIndex;
                }

                if (targetChair >= 0)
                {
                    doctorTarget = Players.FirstOrDefault(p => p.ChairIndex == targetChair);
                }
            }

            // Resolve Night Outcome
            if (mafiaTarget != null)
            {
                if (mafiaTarget == doctorTarget)
                {
                    OnAnnouncement?.Invoke("Ночь прошла спокойно! Доктор спас жертву мафии.");
                }
                else
                {
                    EliminatePlayer(mafiaTarget, "был убит мафией этой ночью!");
                }
            }
            else
            {
                OnAnnouncement?.Invoke("Ночь прошла тихо. Никто не пострадал.");
            }

            if (CheckWinCondition()) return;

            CurrentRound++;
            SetPhase(EGamePhase.DayDiscussion);
        }
        #endregion

        #region Elimination & Win Conditions
        private void EliminatePlayer(MafiaPlayerState player, string reason)
        {
            player.IsAlive = false;

            // Stand up character in 3D world
            if (_tableController != null && _tableController.Table != null)
            {
                Chair chair = _tableController.Table.GetChair(player.ChairIndex);
                if (chair != null)
                {
                    chair.StandUp();
                }
            }

            OnPlayerEliminated?.Invoke(player, $"{player.Name} {reason}\nРоль: {player.GetRoleName()}");
        }

        private bool CheckWinCondition()
        {
            int livingMafia = Players.Count(p => p.IsAlive && p.Role == EPlayerRole.Mafia);
            int livingCivilians = Players.Count(p => p.IsAlive && p.Role != EPlayerRole.Mafia);

            if (livingMafia == 0)
            {
                // Civilians Win
                SetPhase(EGamePhase.GameOver);
                OnGameOver?.Invoke(EPlayerRole.Civilian);
                return true;
            }
            else if (livingMafia >= livingCivilians)
            {
                // Mafia Wins
                SetPhase(EGamePhase.GameOver);
                OnGameOver?.Invoke(EPlayerRole.Mafia);
                return true;
            }

            return false;
        }
        #endregion
    }
}
