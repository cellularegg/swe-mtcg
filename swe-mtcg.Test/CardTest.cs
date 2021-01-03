using System.Runtime.InteropServices;
using NUnit.Framework;
using swe_mtcg.Card;

namespace swe_mtcg.Test
{
    public class CardTest
    {
        [Test]
        public void TestSpellCardGuid()
        {
            //Arrange
            string expectedGuid = "b2237eca-0271-43bd-87f6-b22f70d42ca4";
            ICard spellCard = new SpellCard("Spell", 100, id: expectedGuid);
            //Act 
            var actualGuid = spellCard.Id.ToString();
            //Assert
            Assert.AreEqual(expectedGuid, actualGuid);
            Assert.AreEqual(CardElement.Normal, spellCard.Element);
        }

        [Test]
        public void TestMonsterCardGuid()
        {
            //Arrange
            string expectedGuid = "b2237eca-0271-43bd-87f6-b22f70d42ca4";
            ICard monsterCard = new MonsterCard("Monster", 100, id: expectedGuid);
            //Act 
            MonsterCardCreatureType actualCreatureType = MonsterCardCreatureType.Kraken;
            if (monsterCard is MonsterCard mc)
            {
                actualCreatureType = mc.CreatureType;
            }

            //Assert
            Assert.AreEqual(expectedGuid, monsterCard.Id.ToString());
            Assert.AreEqual(MonsterCardCreatureType.Human, actualCreatureType);
            Assert.AreEqual(CardElement.Normal, monsterCard.Element);
        }

        [Test]
        public void TestCardDamageValue()
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

        [Test]
        public void TestGenericCardCreation()
        {
            // Arrange
            // Spell Cards
            ICard waterSpellCard = AbstractCard.CreateCard("Water Spell", 100);
            ICard fireSpellCard = AbstractCard.CreateCard("Fire Spell", 100);
            ICard normalSpellCard1 = AbstractCard.CreateCard("Spell", 100);
            ICard normalSpellCard2 = AbstractCard.CreateCard("Normal Spell", 100);
            // Monster Cards
            ICard normalHuman1 = AbstractCard.CreateCard("Carlos", 100);
            ICard normalHuman2 = AbstractCard.CreateCard("Normal Human named Carlos", 100);
            ICard waterHuman = AbstractCard.CreateCard("Water Peter", 100);
            ICard fireHuman = AbstractCard.CreateCard("Fire Steve", 100);
            // Assuming when different elements work with humans they work with other creature types as well
            ICard goblin = AbstractCard.CreateCard("Fire Goblin", 100);
            ICard wizard = AbstractCard.CreateCard("Wizard of Oz", 100);
            ICard dragon = AbstractCard.CreateCard("Water Dragon", 100);
            ICard ork = AbstractCard.CreateCard("Ork King", 100);
            ICard knight = AbstractCard.CreateCard("Fire Knight", 100);
            ICard kraken = AbstractCard.CreateCard("Sea Kraken", 100);
            ICard fireElv = AbstractCard.CreateCard("Fire Elv Warrior", 100);

            // Act 
            // ???

            // Assert
            // Spell Cards + Elements 
            Assert.AreEqual(CardElement.Water, waterSpellCard.Element);
            Assert.That(waterSpellCard, Is.TypeOf<SpellCard>());
            Assert.AreEqual(CardElement.Fire, fireSpellCard.Element);
            Assert.That(fireSpellCard, Is.TypeOf<SpellCard>());
            Assert.AreEqual(CardElement.Normal, normalSpellCard1.Element);
            Assert.That(normalSpellCard1, Is.TypeOf<SpellCard>());
            Assert.AreEqual(CardElement.Normal, normalSpellCard2.Element);
            Assert.That(normalSpellCard2, Is.TypeOf<SpellCard>());

            Assert.That(normalHuman1, Is.TypeOf<MonsterCard>());
            MonsterCard tmp = (MonsterCard) normalHuman1;
            Assert.AreEqual(CardElement.Normal, normalHuman1.Element);
            Assert.AreEqual(MonsterCardCreatureType.Human, tmp.CreatureType);

            Assert.That(normalHuman2, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) normalHuman2;
            Assert.AreEqual(CardElement.Normal, normalHuman2.Element);
            Assert.AreEqual(MonsterCardCreatureType.Human, tmp.CreatureType);

            Assert.That(waterHuman, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) waterHuman;
            Assert.AreEqual(CardElement.Water, waterHuman.Element);
            Assert.AreEqual(MonsterCardCreatureType.Human, tmp.CreatureType);

            Assert.That(fireHuman, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) fireHuman;
            Assert.AreEqual(CardElement.Fire, fireHuman.Element);
            Assert.AreEqual(MonsterCardCreatureType.Human, tmp.CreatureType);

            Assert.That(goblin, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) goblin;
            Assert.AreEqual(CardElement.Fire, goblin.Element);
            Assert.AreEqual(MonsterCardCreatureType.Goblin, tmp.CreatureType);

            Assert.That(wizard, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) wizard;
            Assert.AreEqual(CardElement.Normal, wizard.Element);
            Assert.AreEqual(MonsterCardCreatureType.Wizard, tmp.CreatureType);

            Assert.That(dragon, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) dragon;
            Assert.AreEqual(CardElement.Water, dragon.Element);
            Assert.AreEqual(MonsterCardCreatureType.Dragon, tmp.CreatureType);

            Assert.That(ork, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) ork;
            Assert.AreEqual(CardElement.Normal, normalHuman2.Element);
            Assert.AreEqual(MonsterCardCreatureType.Ork, tmp.CreatureType);

            Assert.That(knight, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) knight;
            Assert.AreEqual(CardElement.Fire, knight.Element);
            Assert.AreEqual(MonsterCardCreatureType.Knight, tmp.CreatureType);

            Assert.That(kraken, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) kraken;
            Assert.AreEqual(CardElement.Normal, kraken.Element);
            Assert.AreEqual(MonsterCardCreatureType.Kraken, tmp.CreatureType);

            Assert.That(fireElv, Is.TypeOf<MonsterCard>());
            tmp = (MonsterCard) fireElv;
            Assert.AreEqual(CardElement.Fire, fireElv.Element);
            Assert.AreEqual(MonsterCardCreatureType.FireElv, tmp.CreatureType);
        }

        [Test]
        public void TestSpellCardEffectiveness()
        {
            // Arrange
            SpellCard spellCardFire = new SpellCard("FireCard", 100, CardElement.Fire);
            SpellCard spellCardWater = new SpellCard("WaterCard", 100, CardElement.Water);
            SpellCard spellCardNormal = new SpellCard("NormalCard", 100, CardElement.Normal);
            MonsterCard monsterCardFire = new MonsterCard("FireMonsterCard", 100, CardElement.Fire);
            MonsterCard monsterCardWater = new MonsterCard("WaterMonsterCard", 100, CardElement.Water);
            MonsterCard monsterCardNormal = new MonsterCard("NormalMonsterCard", 100, CardElement.Normal);


            double expectedWaterVsFire = 2.0;
            double expectedWaterVsWater = 1.0;
            double expectedWaterVsNormal = 0.5;
            double expectedFireVsWater = 0.5;
            double expectedFireVsFire = 1.0;
            double expectedFireVsNormal = 2.0;
            double expectedNormalVsWater = 2.0;
            double expectedNormalVsFire = 0.5;
            double expectedNormalVsNormal = 1.0;
            // Act
            double actualWaterVsFire = spellCardWater.GetEffectivenessMultiplier(monsterCardFire);
            double actualWaterVsWater = spellCardWater.GetEffectivenessMultiplier(monsterCardWater);
            double actualWaterVsNormal = spellCardWater.GetEffectivenessMultiplier(spellCardNormal);
            double actualFireVsWater = spellCardFire.GetEffectivenessMultiplier(monsterCardWater);
            double actualFireVsFire = spellCardFire.GetEffectivenessMultiplier(spellCardFire);
            double actualFireVsNormal = spellCardFire.GetEffectivenessMultiplier(spellCardNormal);
            double actualNormalVsWater = spellCardNormal.GetEffectivenessMultiplier(monsterCardWater);
            double actualNormalVsFire = spellCardNormal.GetEffectivenessMultiplier(spellCardFire);
            double actualNormalVsNormal = spellCardNormal.GetEffectivenessMultiplier(monsterCardNormal);
            // Assert
            Assert.AreEqual(expectedWaterVsFire, actualWaterVsFire);
            Assert.AreEqual(expectedWaterVsWater, actualWaterVsWater);
            Assert.AreEqual(expectedWaterVsNormal, actualWaterVsNormal);
            Assert.AreEqual(expectedFireVsWater, actualFireVsWater);
            Assert.AreEqual(expectedFireVsFire, actualFireVsFire);
            Assert.AreEqual(expectedFireVsNormal, actualFireVsNormal);
            Assert.AreEqual(expectedNormalVsWater, actualNormalVsWater);
            Assert.AreEqual(expectedNormalVsFire, actualNormalVsFire);
            Assert.AreEqual(expectedNormalVsNormal, actualNormalVsNormal);
        }

        [Test]
        public void TestCardGetAttackValue()
        {
            // Orks cannot damage Wizard
            // Water Spell instantly defeats Knight
            // Kraken instantly defeats spells
            // Dragons cannot Damage FireElves

            // Arrange
            // Goblins cannot damage Dragon
            MonsterCard goblin = new MonsterCard("Goblin", 1000, CardElement.Normal, MonsterCardCreatureType.Goblin);
            MonsterCard dragonForGoblin =
                new MonsterCard("Dragon", 1, CardElement.Normal, MonsterCardCreatureType.Dragon);

            // Orks cannot damage Wizard
            MonsterCard ork = new MonsterCard("Ork", 1000, CardElement.Normal, MonsterCardCreatureType.Ork);
            MonsterCard wizard = new MonsterCard("Wizard", 1, CardElement.Normal, MonsterCardCreatureType.Wizard);

            // Kraken instantly defeats spells
            MonsterCard kraken = new MonsterCard("Kraken", 0, CardElement.Water, MonsterCardCreatureType.Kraken);
            SpellCard krakenSpell = new SpellCard("Spell", 10000, CardElement.Water);

            // Dragons cannot Damage FireElves
            MonsterCard dragonForFireElv =
                new MonsterCard("Dragon", 1000, CardElement.Fire, MonsterCardCreatureType.Dragon);
            MonsterCard fireElv = new MonsterCard("Fire Elv", 10, CardElement.Fire, MonsterCardCreatureType.FireElv);

            // Monster only
            MonsterCard waterGoblin =
                new MonsterCard("Water Goblin", 10, CardElement.Water, MonsterCardCreatureType.Goblin);
            MonsterCard fireKnight =
                new MonsterCard("Fire Knight", 15, CardElement.Fire, MonsterCardCreatureType.Knight);

            // Spell Only
            // Only need to check one multiplier since GetEffectivenessMultiplier is tested
            SpellCard fireSpell = new SpellCard("Fire Spell", 10, CardElement.Fire);
            SpellCard waterSpell = new SpellCard("Water Spell", 20, CardElement.Water);
            
            // Spell + Monster
            SpellCard normalSpell = new SpellCard("Normal Spell", 10, CardElement.Normal);
            MonsterCard normalKnight = new MonsterCard("normal Knight", 15, CardElement.Normal);
            // Act

            // Assert
            // Goblins cannot damage Dragon
            Assert.AreEqual(0, goblin.GetAttackValue(dragonForGoblin));
            Assert.AreEqual(1, dragonForGoblin.GetAttackValue(goblin));

            // Orks cannot damage Wizard
            Assert.AreEqual(0, ork.GetAttackValue(wizard));
            Assert.AreEqual(1, wizard.GetAttackValue(ork));

            // Kraken instantly defeats spells
            Assert.AreEqual(double.MaxValue, kraken.GetAttackValue(krakenSpell));
            Assert.AreEqual(10000, krakenSpell.GetAttackValue(kraken));

            // Dragons cannot Damage FireElves
            Assert.AreEqual(0, dragonForFireElv.GetAttackValue(fireElv));
            Assert.AreEqual(10, fireElv.GetAttackValue(dragonForFireElv));
            
            // Monster only - attack value should not change
            Assert.AreEqual(10, waterGoblin.GetAttackValue(fireKnight));
            Assert.AreEqual(15, fireKnight.GetAttackValue(waterGoblin));
            
            // Spell only - check if multiplier is applied correctly
            Assert.AreEqual(5, fireSpell.GetAttackValue(waterSpell));
            Assert.AreEqual(40, waterSpell.GetAttackValue(fireSpell));
            
            // Spell + Monster
            // Firespell + Watermonster
            Assert.AreEqual(5,fireSpell.GetAttackValue(waterGoblin));
            Assert.AreEqual(20, waterGoblin.GetAttackValue(fireSpell));
            
            // Waterspell + Watermonster
            Assert.AreEqual(10,waterGoblin.GetAttackValue(waterSpell));
            Assert.AreEqual(20,waterSpell.GetAttackValue(waterGoblin));
            
            // Regularspell + Watermonster
            Assert.AreEqual(10, normalSpell.GetAttackValue(normalKnight));
            Assert.AreEqual(15, normalKnight.GetAttackValue(normalSpell));
        }
    }
}