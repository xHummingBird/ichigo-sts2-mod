using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

// Lasts the entire battle.
public sealed class ReiatsuAbsorptionPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not SpiritualFracturePower && power is not BleedPower || amount <= 0 || applier != Owner)
            return;

        ReiryokuManager.GainReiryoku(Owner.Player, (int)Amount);
    }
}
