namespace swe_mtcg.Card
{
    public class SpellCard : AbstractCard
    {
        public SpellCard(string name, double damage, CardElement element = CardElement.Normal, string id = "") : base(
            name, damage, element, id)
        {
        }

        public double GetEffectivenessMultiplier(ICard other)
        {
            if (this.Element == other.Element)
            {
                return 1;
            }

            if ((this.Element == CardElement.Water && other.Element == CardElement.Fire) ||
                (this.Element == CardElement.Fire && other.Element == CardElement.Normal) ||
                (this.Element == CardElement.Normal && other.Element == CardElement.Water))
            {
                return 2;
            }

            return 0.5;
        }
    }
}