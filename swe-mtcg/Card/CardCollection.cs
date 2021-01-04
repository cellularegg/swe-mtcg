using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace swe_mtcg.Card
{
    public class CardCollection : ICardCollection
    {
        private int _maxCapacity;
        private ConcurrentDictionary<string, ICard> _cards;
        public IReadOnlyDictionary<string, ICard> Cards => _cards as IReadOnlyDictionary<string, ICard>;

        public CardCollection(int maxCapacity = -1)
        {
            _cards = new ConcurrentDictionary<string, ICard>();
            _maxCapacity = maxCapacity;
        }

        public bool AddCard(ICard c)
        {
            if (c == null)
            {
                return false;
            }

            if (_maxCapacity < 0)
            {
                return _cards.TryAdd(c.Id.ToString(), c);
            }

            lock (_cards)
            {
                if (_maxCapacity > _cards.Count)
                {
                    return _cards.TryAdd(c.Id.ToString(), c);
                }

                return false;
            }
        }

        public ICard RemoveCard(string id)
        {
            return _cards.TryRemove(id, out ICard c) ? c : null;
        }

        public ICard PopCard()
        {
            // Maybe not the best solution, should be sufficient
            try
            {
                return _cards.TryRemove(_cards.First().Key, out ICard c) ? c : null;
            }
            // Catch exception when _cards has no elements
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}