using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

public sealed class GetsugaTenshouCounterPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card is not IGetsugaTenshouCard)
            return;

        await PowerCmd.ModifyAmount(choiceContext, this, 1m, Owner, cardPlay.Card);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 2m, Owner, cardPlay.Card);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
            return;

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -2m * Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}
