using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Ichigo.IchigoCode.Cards.Rare;
using Ichigo.IchigoCode.Cards.Uncommon;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Powers;


public class RebornPowerPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card is not IGetsugaCard)
            return;

            Flash();
		    await PowerCmd.Apply<BleedPower>(choiceContext, cardPlay.Target, Amount, Owner, null);
    }
}