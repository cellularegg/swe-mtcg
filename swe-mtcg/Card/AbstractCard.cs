using System;
using System.Net.NetworkInformation;

namespace swe_mtcg.Card
{
    public abstract class AbstractCard : ICard
    {
        public Guid Id { get; }
        public string Name { get; }
        public double Damage { get; }
        public CardElement Element { get; }

        protected AbstractCard(string name, double damage, CardElement element, string id = "")
        {
            this.Name = name;
            this.Damage = Math.Max(damage, 0);
            this.Element = element;
            Id = Guid.TryParse(id, out var tmp) ? tmp : new Guid();
        }

        public static ICard CreateCard(string name, double damage, string id = "")
        {
            // Check if name contains spell
            string lowerName = name.ToLower();
            // Figure out Card Element
            CardElement cardElement = CardElement.Normal;
            if (lowerName.Contains("fire"))
            {
                cardElement = CardElement.Fire;
            }

            if (lowerName.Contains("water"))
            {
                cardElement = CardElement.Water;
            }

            // Default spell type: Normal
            if (lowerName.Contains("spell"))
            {
                return new SpellCard(name, damage, cardElement, id);
            }
            // Default is Monster Card
            else
            {
                MonsterCardCreatureType monsterCardCreatureType = MonsterCardCreatureType.Human;
                if (lowerName.Contains("goblin"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.Goblin;
                }

                else if (lowerName.Contains("wizard"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.Wizard;
                }

                else if (lowerName.Contains("dragon"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.Dragon;
                }

                else if (lowerName.Contains("ork"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.Ork;
                }

                else if (lowerName.Contains("knight"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.Knight;
                }

                else if (lowerName.Contains("firelv") || lowerName.Contains("fire elv"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.FireElv;
                }
                else if (lowerName.Contains("kraken"))
                {
                    monsterCardCreatureType = MonsterCardCreatureType.Kraken;
                }

                return new MonsterCard(name, damage, cardElement, monsterCardCreatureType, id);
            }
        }

        public float GetAttackValue(ICard other)
        {
            // TODO Implement Battle Logic
            throw new NotImplementedException();
        }
    }
}