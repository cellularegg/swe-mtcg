using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace swe_mtcg.Card
{
    public interface ICardDeck
    {
        public List<ICard> Cards { get; }
        public bool AddCard(ICard card);
        public bool RemoveCard(int idx);
        public ICard PopCard(int idx);
    }
}
