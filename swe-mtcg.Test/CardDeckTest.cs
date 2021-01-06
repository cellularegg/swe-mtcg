using NUnit.Framework;
using swe_mtcg.Card;

namespace swe_mtcg.Test
{
    public class CardDeckTest
    {
       [Test]
        public void TestCardDeckAddCard()
        {
            //Arrange
            int maxCardAmount = 1;
            ICardCollection cardDeck = new CardCollection(maxCardAmount);
            //Act 
            var actualSpellHasSuceeded = cardDeck.AddCard(new SpellCard("Spell", 1));
            var actualMonsterHasSuceeded = cardDeck.AddCard(new MonsterCard("Monster", 1));
            //Assert
            Assert.AreEqual(maxCardAmount, cardDeck.Cards.Count);
            Assert.IsFalse(actualMonsterHasSuceeded);
            Assert.IsTrue(actualSpellHasSuceeded);
        }


        [Test]
        public void TestCardDeckRemoveCard()
        {
            //Arrange
            ICardCollection cardDeck = new CardCollection();
            ICard card1 = new SpellCard("Spell", 199);
            ICard card2 = new MonsterCard("Monster", 199);
            cardDeck.AddCard(card1);
            cardDeck.AddCard(card2);
            //Act 
            var actualCardRemoval1HasSucceeded = cardDeck.RemoveCard(card1.Id.ToString());
            var actualCardRemoval2HasSucceeded = cardDeck.RemoveCard("asdasd");
            var actualCard1IsInList = cardDeck.Cards.ContainsKey(card1.Id.ToString());
            var actualCard2IsInList = cardDeck.Cards.ContainsKey(card2.Id.ToString());
            // Assert
            Assert.NotNull(actualCardRemoval1HasSucceeded);
            Assert.Null(actualCardRemoval2HasSucceeded);
            Assert.IsFalse(actualCard1IsInList);
            Assert.IsTrue(actualCard2IsInList);
        }


        [Test]
        public void TestCardDeckPopCard()
        {
            // Arrange
            ICardCollection cardDeck = new CardCollection();
            ICard card1 = new SpellCard("Spell", 199);
            ICard card2 = new MonsterCard("Monster", 199);
            cardDeck.AddCard(card1);
            cardDeck.AddCard(card2);
            // Act
            ICard actualPoppedCard1 = cardDeck.PopCard();
            ICard actualPoppedCard2 = cardDeck.PopCard();
            ICard actualPoppedCardNull = cardDeck.PopCard();
            // Assert
            Assert.IsNull(actualPoppedCardNull);
            Assert.IsNotNull(actualPoppedCard1);
            Assert.IsNotNull(actualPoppedCard2);
            // Fails sometimes becasue .First() element is not always the same
            // Assert.AreEqual(card2, actualPoppedCard2);
            // Assert.AreEqual(card1, actualPoppedCard1);
        } 
    }
}