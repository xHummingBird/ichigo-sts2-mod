using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

// Amount is the number of player turns remaining; expires at the end of the turn it was applied.
public sealed class GetsugaTenshouCounterPower : IchigoPower
{
    private const decimal StrengthPerGetsuga = 1m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (card.Owner.Creature != Owner || card is not IGetsugaCard)
            return;

        await PowerCmd.Apply<GetsugaInfusionPower>(choiceContext, Owner, StrengthPerGetsuga, Owner, card);
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
