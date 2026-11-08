using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Relics;

public abstract class SummonRelicBase : IchigoRelic
{
    [SavedProperty]
    public int StoredReiryoku { get; internal set; }

    public virtual int MaxReiryoku => 100;

    protected virtual int ReiryokuPerTurn => 5;

    protected virtual int ReiryokuPerAttack => 3;

    protected virtual int ReiryokuPerGetsugaTenshou => 6;

    protected virtual int BankaiTurns => 3;

    public override bool ShowCounter => true;

    public override int DisplayAmount => StoredReiryoku;

    public override Task BeforeCombatStart()
    {
        ReiryokuManager.Reset(Owner);

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side)
            return;

        ReiryokuManager.GainReiryoku(Owner, ReiryokuPerTurn);

        await CheckBankaiReady(new ThrowingPlayerChoiceContext(), null);
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay
    )
    {
        CardModel card = cardPlay.Card;

        if (card.Owner != Owner)
            return;

        // Getsuga Tenshou cards are attacks too, but only grant their own amount.
        int gain = card switch
        {
            IGetsugaTenshouCard => ReiryokuPerGetsugaTenshou,
            { Type: CardType.Attack } => ReiryokuPerAttack,
            _ => 0
        };

        ReiryokuManager.GainReiryoku(Owner, gain);

        await CheckBankaiReady(choiceContext, card);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        ReiryokuManager.Reset(Owner);

        return Task.CompletedTask;
    }

    internal void SetReiryoku(int value)
    {
        value = Math.Clamp(value, 0, MaxReiryoku);

        if (StoredReiryoku == value)
            return;

        StoredReiryoku = value;
        InvokeDisplayAmountChanged();
    }

    private async Task CheckBankaiReady(PlayerChoiceContext choiceContext, CardModel? card)
    {
        if (!ReiryokuManager.IsFull(Owner))
            return;

        // Bankai cannot be stacked.
        if (ReiryokuManager.IsInBankai(Owner))
            return;

        Flash();
        ReiryokuManager.Reset(Owner);

        await PowerCmd.Apply<BankaiPower>(
            choiceContext,
            Owner.Creature,
            BankaiTurns,
            Owner.Creature,
            card);
    }
}
