namespace swe_mtcg.Card
{
    public class MonsterCard : AbstractCard
    {
        public MonsterCardCreatureType CreatureType { get; }

        public MonsterCard(string name, double damage, CardElement element = CardElement.Normal,
            MonsterCardCreatureType creatureType = MonsterCardCreatureType.Human, string id = "") : base(name, damage,
            element, id)
        {
            this.CreatureType = creatureType;
        }
    }
}