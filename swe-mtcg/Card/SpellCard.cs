namespace swe_mtcg.Card
{
    public class SpellCard : AbstractCard
    {
        public SpellCard(string name, double damage, CardElement element = CardElement.Normal, string id = "") : base(
            name, damage, element, id)
        {
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Element: {Element}, Damage: {Damage}";
        }
    }
}