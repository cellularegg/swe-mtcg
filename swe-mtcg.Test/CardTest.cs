using NUnit.Framework;
using swe_mtcg.Card;

namespace swe_mtcg.Test
{
    public class CardTest
    {
        [Test]
        public void TestSpellCardHasSpellCardType()
        {
            //Arrange
            ICard spellCard = new SpellCard("Spell", 100);
            //Act 
            var actual = spellCard.Type;
            //Assert
            Assert.AreEqual(CardType.SpellCard, actual);

        }

        [Test]
        public void TestMonsterCardHasMonsterCardType()
        {
            //Arrange
            ICard spellCard = new MonsterCard("Monster", 100);
            //Act 
            var actual = spellCard.Type;
            //Assert
            Assert.AreEqual(CardType.MonsterCard, actual);

        }

        [Test]
        public void TestInitialCardValues()
        {
            //Arrange
            ICard mySpellCard = new SpellCard("Spell", -100);
            ICard myMonsterCard = new MonsterCard("Monster", -144);
            //Act 
            var actualSpellCardDamage = mySpellCard.Damage;
            var actualMonsterCardDamage = myMonsterCard.Damage;
            //Assert
            Assert.AreEqual(0, actualMonsterCardDamage);
            Assert.AreEqual(0, actualSpellCardDamage);

        }
    }
}
