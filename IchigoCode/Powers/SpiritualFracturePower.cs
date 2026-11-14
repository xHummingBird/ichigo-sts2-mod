using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

public sealed class SpiritualFracturePower : IchigoPower
{
    private const decimal BaseDamageIncrease = 1.25m;
    private const decimal BankaiDamageIncrease = 1.5m;

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

        if (!props.IsPoweredAttack())
            return 1m;

        Player? attacker = dealer?.Player;
        if (attacker is not null && ReiryokuManager.IsInTrueBankai(attacker))
            return BankaiDamageIncrease;

        return BaseDamageIncrease;
    }
}
