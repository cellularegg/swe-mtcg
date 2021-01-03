using swe_mtcg.Card;
using System.Collections.Generic;

namespace swe_mtcg.Player
{
    public interface IPlayer
    {
        public string Username { get; }
        public List<ICard> CardList { get; }
        public double Elo { get; }
        public double Coins { get;  }
        public bool AddCardToActiveDeck(int idxOfCard);
        public ICard TradeCard(int idxOfCard);
        public void AddCardToCardList(ICard card);
        public bool ChangeCoins(float amount);
        public bool ChangeElo(float amount);
    }
}
