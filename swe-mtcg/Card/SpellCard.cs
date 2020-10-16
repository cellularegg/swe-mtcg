using System;
using System.Collections.Generic;
using System.Text;

namespace swe_mtcg.Card
{
    public class SpellCard : AbstractCard
    {
        public SpellCard(string name, int damage, CardElement element = CardElement.Normal) : base(name, damage, element, CardType.SpellCard)
        {
        }
    }
}
