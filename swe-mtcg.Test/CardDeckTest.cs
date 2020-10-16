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
            Assert.AreEqual(actualMonsterHasSuceeded, false);
            Assert.AreEqual(actualSpellHasSuceeded, true);
        }

    }
}
