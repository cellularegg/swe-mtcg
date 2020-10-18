using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace swe_mtcg.Card
{
    public class CardDeck : ICardDeck
    {
        private int _maxCardAmount;
        private List<ICard> _Cards;
        public IList<ICard> Cards { get { return this._Cards.AsReadOnly(); } }

        public CardDeck(int maxCardAmount = 4)
        {
            _Cards = new List<ICard>();
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
                _Cards.Add(card);
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICard PopCard(int idx = 0)
        {
            throw new NotImplementedException();
        }

        public bool RemoveCard(int idx)
        {
            // Check if emtpy or idx out of range
            if (Cards.Count == 0 || (Cards.Count - 1) < idx)
            {
                return false;
            }
            else
            {
                _Cards.RemoveAt(idx);
                return true;
            }
        }
    }
}