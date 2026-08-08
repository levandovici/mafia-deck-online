using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mafia
{
    [Serializable]
    public class CardData
    {
        public ECardType Type;
        public string Text;

        public CardData(ECardType type, string text)
        {
            Type = type;
            Text = text;
        }

        public Color GetColor()
        {
            switch (Type)
            {
                case ECardType.Accusation:
                    return new Color(0.9f, 0.25f, 0.25f, 1f); // Red
                case ECardType.Defense:
                    return new Color(0.25f, 0.8f, 0.35f, 1f); // Green
                case ECardType.Analysis:
                    return new Color(0.95f, 0.8f, 0.2f, 1f);  // Yellow
                default:
                    return Color.white;
            }
        }
    }

    public static class CardDeck
    {
        private static readonly string[] AccusationTexts = new string[]
        {
            "Подозрительно",
            "Я голосую против",
            "Ты мафия",
            "Он врёт",
            "Слишком тихий",
            "Не доверяю",
            "Лжёт в глаза",
            "Мафиози",
            "Убийца",
            "Под килл",
            "Виноват",
            "Секретный план",
            "Не верю",
            "Ловкий мафия",
            "Глаза выдают",
            "Обманщик",
            "Вычеркнуть",
            "Твой конец",
            "Предатель",
            "Убить сейчас"
        };

        private static readonly string[] DefenseTexts = new string[]
        {
            "Я мирный",
            "Это ошибка",
            "Подумайте ещё",
            "Не я",
            "Виноваты другие",
            "Я за мирных",
            "Докажу невиновен",
            "Ложное обвинение",
            "Спасите меня",
            "Чистый",
            "Не трогайте",
            "Я на вашей стороне",
            "Ошибка",
            "Мирный житель",
            "Верю вам",
            "Давайте вместе",
            "Невиновен",
            "Поддержите",
            "Я хороший",
            "Не мафия"
        };

        private static readonly string[] AnalysisTexts = new string[]
        {
            "Он молчал",
            "Слишком активен",
            "Меняет мнение",
            "Странный голос",
            "Смотрит в сторону",
            "Повторяет других",
            "Слишком уверен",
            "Нервничает",
            "Игнорирует вопросы",
            "Подозрительный тайминг",
            "Слабая защита",
            "Активен ночью?",
            "Логика хромает",
            "Скрывает роль",
            "Группируется",
            "Ранний голос",
            "Избегает глаз",
            "Контраргументы",
            "Пассивен",
            "Несостыковки"
        };

        public static CardData GetRandomCard()
        {
            int category = UnityEngine.Random.Range(0, 3);
            switch (category)
            {
                case 0:
                    return new CardData(ECardType.Accusation, AccusationTexts[UnityEngine.Random.Range(0, AccusationTexts.Length)]);
                case 1:
                    return new CardData(ECardType.Defense, DefenseTexts[UnityEngine.Random.Range(0, DefenseTexts.Length)]);
                default:
                    return new CardData(ECardType.Analysis, AnalysisTexts[UnityEngine.Random.Range(0, AnalysisTexts.Length)]);
            }
        }

        public static List<CardData> GetInitialHand(int count = 4)
        {
            List<CardData> hand = new List<CardData>();
            hand.Add(new CardData(ECardType.Accusation, AccusationTexts[UnityEngine.Random.Range(0, AccusationTexts.Length)]));
            hand.Add(new CardData(ECardType.Defense, DefenseTexts[UnityEngine.Random.Range(0, DefenseTexts.Length)]));
            hand.Add(new CardData(ECardType.Analysis, AnalysisTexts[UnityEngine.Random.Range(0, AnalysisTexts.Length)]));
            while (hand.Count < count)
            {
                hand.Add(GetRandomCard());
            }
            return hand;
        }
    }
}
