using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
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
            ICardDeck cardDeck = new CardDeck(maxCardAmount);
            //Act 
            var actualSpellHasSuceeded = cardDeck.AddCard(new SpellCard("Spell", 1));
            var actualMonsterHasSuceeded = cardDeck.AddCard(new MonsterCard("Monster", 1));
            //Assert
            Assert.AreEqual(maxCardAmount, cardDeck.Cards.Count);
            Assert.IsFalse(actualMonsterHasSuceeded);
            Assert.IsTrue(actualSpellHasSuceeded);
        }

        [Test]
        public void TestCardDeckListMethodAccess()
        {
            // Arrange
            ICardDeck cardDeck = new CardDeck();
            ICard card1 = new SpellCard("Spell", 10);
            cardDeck.AddCard(card1);
            // Act + Act
            Assert.Throws<System.NotSupportedException>(
                () => cardDeck.Cards.Add(new SpellCard("Spell", 100))
            );
            Assert.Throws<System.NotSupportedException>(
                () => cardDeck.Cards.Remove(card1)
            );
            Assert.Throws<System.NotSupportedException>(
                () => cardDeck.Cards.Clear()
            );


            Assert.AreEqual(cardDeck.Cards.Count, 1);
        }

        [Test]
        public void TestCardDeckRemoveCard()
        {
            //Arrange
            ICardDeck cardDeck = new CardDeck();
            ICard card1 = new SpellCard("Spell", 199);
            ICard card2 = new MonsterCard("Monster", 199);
            cardDeck.AddCard(card1);
            cardDeck.AddCard(card2);
            //Act 
            var actualCardRemoval1HasSucceeded = cardDeck.RemoveCard(0);
            var actualCardRemoval2HasSucceeded = cardDeck.RemoveCard(10);
            var actualCard1IsInList = cardDeck.Cards.Contains(card1);
            var actualCard2IsInList = cardDeck.Cards.Contains(card2);
            // Assert
            Assert.IsTrue(actualCardRemoval1HasSucceeded);
            Assert.IsFalse(actualCardRemoval2HasSucceeded);
            Assert.IsFalse(actualCard1IsInList);
            Assert.IsTrue(actualCard2IsInList);
        }

    }
}
