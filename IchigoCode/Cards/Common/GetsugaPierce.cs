using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class GetsugaPierce() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy), IGetsugaCard
{
    private bool IsInBankai =>
        IsMutable &&
        Owner != null &&
        (
            Owner.Creature.HasPower<TrueBankaiPower>() ||
            Owner.Creature.HasPower<BankaiHosPower>()
        );
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
        HoverTipFactory.FromPower<BleedPower>()
    ];

    private bool HasPlayedGetsugaTenshouThisTurn =>
        IsMutable &&
        Owner != null &&
        CombatState != null &&
        CombatManager.Instance.History.CardPlaysFinished.Any(
            entry =>
                entry.HappenedThisTurn(CombatState) &&
                entry.CardPlay.Player == Owner &&
                entry.CardPlay.Card is GetsugaTenshou);

    protected override bool ShouldGlowGoldInternal =>
        HasPlayedGetsugaTenshouThisTurn ||
        IsInBankai;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move)
    ];

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card != this)
            return false;

        if (card.Pile?.Type is not
            (PileType.Hand or PileType.Play))
        {
            return false;
        }

        if (!HasPlayedGetsugaTenshouThisTurn)
            return false;

        modifiedCost = 0m;

        return modifiedCost != originalCost;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null ||
            target == null)
        {
            return;
        }

        var isInBankai = IsInBankai;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (Owner.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomGetsuga();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 550f,
                durationSeconds: 0.2f,
                overrideAnim: "point_getsuga");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            await Task.Delay(33);

            SfxCmd.Play(
                "res://Ichigo/sfx/getsuga_6.wav",
                0.8f);

            await Task.Delay(17);

            ichigo.PlayVfxOnTarget(
                target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit");

            await DamageCmd.Attack(
                    DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
                .Execute(choiceContext);

            await ichigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
        else
        {
            await DamageCmd.Attack(
                    DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
                .Execute(choiceContext);
        }

        if (isInBankai)
        {
            var bleedPower =
                target.GetPower<BleedPower>();

            if (bleedPower != null)
            {
                await bleedPower.Trigger();
            }
        }

        if (!target.HasPower<SpiritualFracturePower>())
        {
            await PowerCmd.Apply<SoulDisruptionPower>(
                choiceContext,
                target,
                1m,
                ownerCreature,
                this);
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}