using System;
using System.Net;
using swe_mtcg.Card;

namespace swe_mtcg.User
{
    public class TradingDeal
    {
        public Guid Id { get; private set; }
        public ICard CardToTrade { get; private set; }
        public string Owner { get; private set; }
        public string TypeRequirement { get; private set; }
        public double DamageRequirement { get; private set; }
        public double EloRequirement { get; private set; }

        public TradingDeal(ICard cardToTrade, string owner, string typeRequirement, double damageRequirement,
            double eloRequirement = Double.MaxValue, string id = "")
        {
            CardToTrade = cardToTrade;
            Owner = owner;
            TypeRequirement = typeRequirement;
            DamageRequirement = damageRequirement;
            EloRequirement = eloRequirement;
            Id = Guid.TryParse(id, out Guid tmp) ? tmp : Guid.NewGuid();
        }

        public bool IsValidTrade(ICard other)
        {
            if ((other.Damage < DamageRequirement) || CardToTrade.Id == other.Id)
            {
                return false;
            }
            // TypeRequirement can be Monster, Spell, Fire, Water, Normal
            switch (TypeRequirement.ToLower())
            {
                case "monster":
                    if (other is MonsterCard)
                    {
                        return true;
                    }
                    break;
                case "spell":
                    if (other is SpellCard)
                    {
                        return true;
                    }
                    break;
                default:
                    if (Enum.TryParse<CardElement>(TypeRequirement, true, out CardElement cardElement))
                    {
                        if (other.Element == cardElement)
                        {
                            return true;
                        }
                    }

                    return false;
            }
            return false;
        }
    }
}