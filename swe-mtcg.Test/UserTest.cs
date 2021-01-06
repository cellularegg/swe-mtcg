using System;
using NUnit.Framework;
using swe_mtcg.Card;

namespace swe_mtcg.Test
{
    public class UserTest
    {

        [Test]
        public void TestCreateNewUser()
        {
            User.User u1 = new User.User("admin", "passwort");
            Assert.Throws<ArgumentException>(() => new User.User("", "passwort"));
            Assert.Throws<ArgumentException>(() => new User.User("usär", "passwort"));
            Assert.Throws<ArgumentException>(() => new User.User("##aabb##?", "passwort"));
 
        }

        [Test]
        public void TestMoveCardToDeck()
        {
            // Create User
            User.User u = new User.User("admin", "passwort");

            // Create Card
            ICard c = AbstractCard.CreateCard("Normal Spell", 100);
            string cardId = c.Id.ToString();

            // Card not in stack yet
            Assert.IsFalse(u.MoveCardToDeck(cardId));
            u.Stack.AddCard(c);
            Assert.IsTrue(u.MoveCardToDeck(cardId));
            Assert.AreEqual(0, u.Stack.Cards.Count);
            Assert.IsFalse(u.MoveCardToDeck(cardId));
        }

        [Test]
        public void TestMoveCardToStack()
        {
            // Create User
            User.User u = new User.User("admin", "passwort");

            // Create Card
            ICard c = AbstractCard.CreateCard("Normal Spell", 100);
            string cardId = c.Id.ToString();

            // Card not in stack yet
            Assert.IsFalse(u.MoveCardToStack(cardId));
            u.Deck.AddCard(c);
            Assert.IsTrue(u.MoveCardToStack(cardId));
            Assert.AreEqual(0, u.Deck.Cards.Count);
            Assert.IsFalse(u.MoveCardToStack(cardId));
        }

        [Test]
        public void TestWin()
        {
            User.User u = new User.User("admin", "passwort");
            u.Win();
            Assert.AreEqual(105, u.Elo);
            Assert.AreEqual(1, u.Wins);
            Assert.AreEqual(0, u.Looses);
            Assert.AreEqual(1, u.TotalGames);
        }

        [Test]
        public void TestLoose()
        {
            User.User u = new User.User("admin", "passwort");
            u.Loose();
            Assert.AreEqual(97, u.Elo);
            Assert.AreEqual(1, u.Looses);
            Assert.AreEqual(0, u.Wins);
            Assert.AreEqual(1, u.TotalGames);
        }

        [Test]
        public void TestDraw()
        {
            User.User u = new User.User("admin", "passwort");
            u.Draw();
            Assert.AreEqual(100, u.Elo);
            Assert.AreEqual(0, u.Looses);
            Assert.AreEqual(0, u.Wins);
            Assert.AreEqual(1, u.TotalGames);
        }
    }
}