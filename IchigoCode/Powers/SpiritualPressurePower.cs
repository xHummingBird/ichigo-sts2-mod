using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Ichigo.IchigoCode.Powers;

// Does nothing on its own; at the threshold it is consumed and becomes Spiritual Fracture.
public sealed class SpiritualPressurePower : IchigoPower
{
    private const int FractureThreshold = 3;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power != this || Amount < FractureThreshold)
            return;

        await PowerCmd.Remove(this);

        if (!Owner.HasPower<SpiritualFracturePower>())
        {
            await PowerCmd.Apply<SpiritualFracturePower>(choiceContext, Owner, 1, applier, cardSource);
        }
    }
}
