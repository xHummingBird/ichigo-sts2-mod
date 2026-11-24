using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;

namespace Ichigo.IchigoCode.Powers;

public sealed class ShunpoMasteryCounterPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card is not IGetsugaTenshouCard)
            return;

        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
    }
}
