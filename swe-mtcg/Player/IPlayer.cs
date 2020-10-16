using swe_mtcg.Card;
using System;
using System.Collections.Generic;
using System.Text;

namespace swe_mtcg.Player
{
    public interface IPlayer
    {
        public string Username { get; }
        public List<ICard> CardList { get; }
        public float Elo { get; }
        public ICardDeck ActiveDeck { get; }
        public float Coins { get;  }
        public bool AddCardToActiveDeck(int idxOfCard);
        public ICard TradeCard(int idxOfCard);
        public void AddCardToCardList(ICard card);
        public bool ChangeCoins(float amount);
        public bool ChangeElo(float amount);
    }
}
