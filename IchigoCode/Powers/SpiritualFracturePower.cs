using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Relics;
using MegaCrit.Sts2.Core.Commands;

namespace Ichigo.IchigoCode.Powers;

public sealed class SpiritualFracturePower : IchigoPower
{
    private const decimal BaseDamageIncrease = 1.25m;
    private const decimal BankaiDamageIncrease = 1.5m;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

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

        decimal increase =
            attacker is not null &&
            ReiryokuManager.IsInTrueBankai(attacker)
                ? BankaiDamageIncrease
                : BaseDamageIncrease;

        if (attacker?.GetRelic<HollowMask>() != null)
            increase += 0.25m;

        if (dealer is not null &&
            dealer.HasPower<PowerNullificationPower>())
        {
            increase =
                1m +
                (increase - 1m) * 2m;
        }

        return increase;
    }

    public override async Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        if (Owner.HasPower<SoulDisruptionPower>())
            await PowerCmd.Remove(Owner.GetPower<SoulDisruptionPower>());
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return;
        
        await PowerCmd.Decrement(this);
    }

}
