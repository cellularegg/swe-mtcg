using System;
using System.Collections.Generic;
using System.Text;

namespace swe_mtcg.Card
{
    public interface ICard
    {
        string Name { get; }
        int Damage { get; }
        CardElement Element { get; }
        CardType Type { get; }
    }
}
