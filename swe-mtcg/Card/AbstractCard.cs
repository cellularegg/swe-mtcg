using System;
using System.Collections.Generic;
using System.Text;

namespace swe_mtcg.Card
{
    public abstract class AbstractCard : ICard
    {
        public string Name { get; }
        public int Damage { get; }
        public CardElement Element { get; }
        public CardType Type { get; }
        public AbstractCard(string name, int damage, CardElement element, CardType type)
        {
            this.Name = name;
            if (damage < 0)
            {
                this.Damage = 0;
            }
            else
            {
                this.Damage = damage;
            }
            this.Element = element;
            this.Type = type;
        }
    }
}
