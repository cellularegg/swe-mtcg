using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace swe_mtcg.Card
{
    public class MonsterCard : AbstractCard
    {
        public MonsterCardCreatureType CreatureType { get; }
        public MonsterCard(string name, int damage, CardElement element = CardElement.Normal, MonsterCardCreatureType creatureType = MonsterCardCreatureType.Human ) : base(name, damage, element, CardType.MonsterCard)
        {
            this.CreatureType = creatureType;
        }
    }
}
