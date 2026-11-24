using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Powers;

public sealed class TheResolveToKillPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _GetsugaTenshouBoostUsed;

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is IGetsugaTenshouCard && !card.Keywords.Contains(CardKeyword.Retain))
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        if (card is IGetsugaTenshouCard && !card.Keywords.Contains(CardKeyword.Retain))
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_GetsugaTenshouBoostUsed && cardPlay.Card is IGetsugaTenshouCard && cardPlay.Card.Owner.Creature == Owner)
            _GetsugaTenshouBoostUsed = true;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
            return;

        _GetsugaTenshouBoostUsed = false;
        await PowerCmd.Decrement(this);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        {
            if (!_GetsugaTenshouBoostUsed && cardSource is IGetsugaTenshouCard && dealer == Owner)
                return 6m;
            return 0m;
        }
}
