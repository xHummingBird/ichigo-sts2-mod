using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ichigo.IchigoCode.Cards.Token;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Powers;

public sealed class TheResolveToKillPower : IchigoPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<GetsugaTenshou>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    public override bool TryModifyKeywordsInCombat(
        CardModel card,
        ISet<CardKeyword> keywords)
    {
        if (card.Owner != Owner.Player)
            return false;

        if (card is not GetsugaTenshou)
            return false;

        return keywords.Add(CardKeyword.Retain);
    }

    public override Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        InvokeKeywordsChangedOnGetsugaTenshous(
            Owner.Player);

        return Task.CompletedTask;
    }

    public override Task AfterRemoved(
        Creature oldOwner)
    {
        InvokeKeywordsChangedOnGetsugaTenshous(
            oldOwner.Player);

        return Task.CompletedTask;
    }

    private static void InvokeKeywordsChangedOnGetsugaTenshous(
        Player player)
    {
        foreach (CardModel card in
                 player.PlayerCombatState.AllCards
                     .Where(card => card is GetsugaTenshou))
        {
            card.InvokeKeywordsChanged();
        }
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack())
            return 0m;

        if (cardSource is not GetsugaTenshou)
            return 0m;

        if (dealer != Owner)
            return 0m;

        var getsugaTenshousPlayedThisTurn =
            CombatManager.Instance.History.CardPlaysFinished.Count(
                (CardPlayFinishedEntry entry) =>
                    entry.HappenedThisTurn(CombatState) &&
                    entry.CardPlay.Card is GetsugaTenshou &&
                    entry.CardPlay.Player == Owner.Player);

        if (getsugaTenshousPlayedThisTurn > 0)
            return 0m;

        return Amount;
    }
}