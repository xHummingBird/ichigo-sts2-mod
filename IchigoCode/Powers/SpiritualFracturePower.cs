using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

public sealed class SpiritualFracturePower : IchigoPower
{
    private const decimal DamageIncrease = 1.5m;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (target != Owner)
            return 1m;

        if (props.IsPoweredAttack() && ReiryokuManager.IsInTrueBankai(Owner))
        {
            if (ReiryokuManager.IsInTrueBankai(Owner))
                return 2m;
            else
                return DamageIncrease;
        }
        return 1m;
    }
}
