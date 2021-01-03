using System.Collections.Concurrent;
using System.Collections.Generic;

namespace swe_mtcg.Card
{
    public interface ICardCollection
    {
        IReadOnlyDictionary<string,ICard> Cards { get; }
        bool AddCard(ICard c);
        ICard RemoveCard(string id);
        ICard PopCard();
    }
}