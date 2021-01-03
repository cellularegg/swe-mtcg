using System;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

namespace swe_mtcg.Card
{
    public abstract class AbstractCard : ICard
    {
        public Guid Id { get; }
        public string Name { get; }
        public double Damage { get; }
        public CardElement Element { get; }

        protected AbstractCard(string name, double damage, CardElement element, string id = "a")
        {
            this.Name = name;
            this.Damage = Math.Max(damage, 0);
            this.Element = element;
            Id = Guid.TryParse(id, out Guid tmp) ? tmp : Guid.NewGuid();
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

        public double GetEffectivenessMultiplier(ICard other)
        {
            if (this.Element == other.Element)
            {
                return 1;
            }

            if ((this.Element == CardElement.Water && other.Element == CardElement.Fire) ||
                (this.Element == CardElement.Fire && other.Element == CardElement.Normal) ||
                (this.Element == CardElement.Normal && other.Element == CardElement.Water))
            {
                return 2;
            }

            return 0.5;
        }

        // Battle of two cards
        // return 1 when card 1 wins
        // return 2 when card 2 wins
        // return 0 when draw
        public static int Battle(ICard card1, ICard card2)
        {
            double card1AttackVal = card1.GetAttackValue(card2);
            double card2AttackVal = card2.GetAttackValue(card1);
            if (card1AttackVal < card2AttackVal)
            {
                return 2;
            }

            if (card1AttackVal > card2AttackVal)
            {
                return 1;
            }

            return 0;
        }

        public double GetAttackValue(ICard other)
        {
            // TODO Implement Battle Logic

            if (this is MonsterCard && other is MonsterCard)
            {
                MonsterCard thisMc = (MonsterCard) this;
                MonsterCard otherMc = (MonsterCard) other;
                // Goblins cannot damage Dragon
                if (thisMc.CreatureType == MonsterCardCreatureType.Goblin &&
                    otherMc.CreatureType == MonsterCardCreatureType.Dragon)
                {
                    return 0;
                }

                // Orks cannot damage Wizard
                if (thisMc.CreatureType == MonsterCardCreatureType.Ork &&
                    otherMc.CreatureType == MonsterCardCreatureType.Wizard)
                {
                    return 0;
                }

                // Dragons cannot Damage FireElves
                if (thisMc.CreatureType == MonsterCardCreatureType.Dragon &&
                    otherMc.CreatureType == MonsterCardCreatureType.FireElv)
                {
                    return 0;
                }

                // Monster only - no multiplier 
                return this.Damage;
            }

            // Kraken instantly defeats spells
            if (this is MonsterCard && other is SpellCard)
            {
                MonsterCard thisMc = (MonsterCard) this;
                if (thisMc.CreatureType == MonsterCardCreatureType.Kraken)
                {
                    return double.MaxValue;
                }
            }

            // Water Spell instantly defeats Knight
            if (this is SpellCard && other is MonsterCard)
            {
                SpellCard thisSc = (SpellCard) this;
                MonsterCard otherMc = (MonsterCard) other;
                if (thisSc.Element == CardElement.Water && otherMc.CreatureType == MonsterCardCreatureType.Knight)
                {
                    return double.MaxValue;
                }
            }

            // Calculate Effectiveness
            double multiplier = this.GetEffectivenessMultiplier(other);
            return multiplier * this.Damage;
        }
    }
}