namespace swe_mtcg.Card
{
    public class SpellCard : AbstractCard
    {
        public SpellCard(string name, double damage, CardElement element = CardElement.Normal, string id = "") : base(
            name, damage, element, id)
        {
        }

        
    }
}