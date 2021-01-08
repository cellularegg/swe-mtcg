using NUnit.Framework;
using swe_mtcg.Card;
using swe_mtcg.User;

namespace swe_mtcg.Test
{
    public class TradingDealTest
    {
        [Test]
        public void TestIsValidTrade()
        {
            ICard monsterKnight = AbstractCard.CreateCard("Monster Knight", 25);
            ICard waterSpell1 = AbstractCard.CreateCard("water Spell", 25);
            ICard waterSpell2 = AbstractCard.CreateCard("water Spell", 1);
            ICard fireSpell = AbstractCard.CreateCard("fire Spell", 100);

            TradingDeal td1 = new TradingDeal(monsterKnight,"admin", "spell", 20);
            TradingDeal td2 = new TradingDeal(monsterKnight,"admin", "monster", 20);
            TradingDeal td3 = new TradingDeal(monsterKnight, "admin", "water", 20);
            Assert.IsTrue(td1.IsValidTrade(waterSpell1));
            // Dont allow trading with "yourself"
            Assert.IsFalse(td2.IsValidTrade(monsterKnight));
            // Valid Water
            Assert.IsTrue(td3.IsValidTrade(waterSpell1));
            Assert.IsFalse(td3.IsValidTrade(fireSpell));
            // Dmg req not fulfilled
            Assert.IsFalse(td3.IsValidTrade(waterSpell2));
            

        }
    }
}