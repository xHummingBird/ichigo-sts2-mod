using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Powers;

public class BlutVenePower : IchigoPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner ||
            dealer == null ||
            !props.IsPoweredAttack() ||
            !dealer.HasPower<SpiritualFracturePower>())
        {
            return;
        }

        Flash();

        await CreatureCmd.Damage(
            choiceContext,
            dealer,
            Amount,
            ValueProp.Unpowered | ValueProp.SkipHurtAnim,
            Owner,
            null,
            null);
    }
}