using System;
using System.Collections.Generic;
using System.Text;

namespace swe_mtcg.Card
{
    public interface ICard
    {
        Guid Id { get; }
        string Name { get; }
        double Damage { get; }
        CardElement Element { get; }
        double GetAttackValue(ICard other);
    }
}
