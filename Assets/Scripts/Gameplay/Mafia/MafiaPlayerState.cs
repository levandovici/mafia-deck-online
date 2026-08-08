using System;

namespace Mafia
{
    [Serializable]
    public class MafiaPlayerState
    {
        public int ChairIndex;
        public string PlayerId;
        public string Name;
        public PlayerData PlayerData;
        public EPlayerRole Role;
        public bool IsAlive;
        public bool IsLocal;
        public bool IsBot;
        public int VotesReceived;

        public MafiaPlayerState(int chairIndex, string playerId, string name, PlayerData playerData, EPlayerRole role, bool isLocal, bool isBot)
        {
            ChairIndex = chairIndex;
            PlayerId = playerId;
            Name = name;
            PlayerData = playerData;
            Role = role;
            IsAlive = true;
            IsLocal = isLocal;
            IsBot = isBot;
            VotesReceived = 0;
        }

        public string GetRoleName()
        {
            switch (Role)
            {
                case EPlayerRole.Mafia: return "Мафия";
                case EPlayerRole.Doctor: return "Доктор";
                case EPlayerRole.Commissioner: return "Комиссар";
                case EPlayerRole.Civilian: return "Мирный житель";
                default: return "Мирный";
            }
        }

        public string GetTeamName()
        {
            return Role == EPlayerRole.Mafia ? "Команда Мафии" : "Мирные жители";
        }

        public string GetRoleDescription()
        {
            switch (Role)
            {
                case EPlayerRole.Mafia: return "Уничтожьте всех мирных жителей. Ночью выбирайте жертву.";
                case EPlayerRole.Doctor: return "Спасайте игроков ночью. Спасите себя или другого.";
                case EPlayerRole.Commissioner: return "Проверяйте подозрительных игроков ночью.";
                case EPlayerRole.Civilian: return "Найдите мафию в дневном обсуждении и проголосуйте за неё.";
                default: return "Вычислите мафию!";
            }
        }
    }
}
