using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;

namespace Ichigo.IchigoCode.Powers;

public sealed class BattleInstinctPower : IchigoPower
{
    private class Data
    {
        public int attacksPlayedThisTurn;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            GetInternalData<Data>().attacksPlayedThisTurn++;
            if (GetInternalData<Data>().attacksPlayedThisTurn == 3)
            {
                await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            GetInternalData<Data>().attacksPlayedThisTurn = 0;
        }
    }
}

