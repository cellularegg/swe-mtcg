using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace swe_mtcg.Card
{
    public class CardDeck : ICardDeck
    {
        private int _maxCardAmount;
        public List<ICard> Cards { get; }

        public CardDeck(int maxCardAmount = 4)
        {
            Cards = new List<ICard>();
            if (maxCardAmount <= 0)
            {
                this._maxCardAmount = 4;
            }
            else
            {
                this._maxCardAmount = maxCardAmount;
            }
        }

        public bool AddCard(ICard card)
        {
            if (_maxCardAmount > Cards.Count)
            {
                Cards.Add(card);
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICard PopCard(int idx)
        {
            throw new NotImplementedException();
        }

        public bool RemoveCard(int idx)
        {
            throw new NotImplementedException();
        }
    }
}
