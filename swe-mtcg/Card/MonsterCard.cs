using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace swe_mtcg.Card
{
    public class MonsterCard : AbstractCard
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MonsterCardCreatureType CreatureType { get; }

        public MonsterCard(string name, double damage, CardElement element = CardElement.Normal,
            MonsterCardCreatureType creatureType = MonsterCardCreatureType.Human, string id = "") : base(name, damage,
            element, id)
        {
            this.CreatureType = creatureType;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Element: {Element}, CreatureType: {CreatureType}, Damage: {Damage}";
        }
    }
}