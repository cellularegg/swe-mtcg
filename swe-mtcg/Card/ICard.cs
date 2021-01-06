using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace swe_mtcg.Card
{
    public interface ICard
    {
        Guid Id { get; }
        string Name { get; }
        double Damage { get; }
        [JsonConverter(typeof(StringEnumConverter))]  
        CardElement Element { get; }
        double GetAttackValue(ICard other);
    }
}
