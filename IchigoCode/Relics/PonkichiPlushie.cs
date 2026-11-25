using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Relics;

public sealed class PonkichiPlushie : IchigoRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (target != Owner.Creature)
            return 1m;

        if (!props.IsPoweredAttack())
            return 1m;

        if (dealer == null ||
            !dealer.HasPower<SpiritualFracturePower>())
        {
            return 1m;
        }

        return 0.75m;
    }
}