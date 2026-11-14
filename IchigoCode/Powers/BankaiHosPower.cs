using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Powers;

public class BankaiHosPower : IchigoPower
{
    private const decimal GetsugaDamageMultiplier = 2m;
    private const decimal FractureBonusDamage = 3m;
    private const int BleedPerHit = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner || target == null)
            return 0m;

        if (!props.IsPoweredAttack())
            return 0m;

        return target.HasPower<SpiritualFracturePower>() ? FractureBonusDamage : 0m;
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner)
            return 1m;

        if (!props.IsPoweredAttack())
            return 1m;

        return cardSource is IGetsugaTenshouCard ? GetsugaDamageMultiplier : 1m;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer != Owner || target == Owner)
            return;

        if (cardSource is not IGetsugaTenshouCard || !props.IsPoweredAttack())
            return;

        if (result.UnblockedDamage <= 0)
            return;

        await PowerCmd.Apply<BleedPower>(choiceContext, target, BleedPerHit, Owner, cardSource);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
            return;

        await PowerCmd.Decrement(this);
    }
}