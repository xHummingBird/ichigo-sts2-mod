using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class BlackGetsuga() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy), IGetsugaCard
{
    private bool IsInBankai =>
        IsMutable &&
        Owner != null &&
        (
            Owner.Creature.HasPower<TrueBankaiPower>() ||
            Owner.Creature.HasPower<BankaiHosPower>()
        );

    protected override bool ShouldGlowGoldInternal =>
        IsInBankai;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
    ];

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card != this)
            return false;

        if (card.Pile?.Type is not (PileType.Hand or PileType.Play))
            return false;

        if (!IsInBankai)
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

        if (ownerCreature == null || target == null)
            return;

        if (Owner.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomGetsuga();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 550f,
                durationSeconds: 0.2f,
                overrideAnim: "black_getsuga");

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
                "hit_black");

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

        var bleedPower =
            target.GetPowerAmount<BleedPower>();

        if (bleedPower != null && bleedPower > 0)
        {
            await PowerCmd.Apply<BleedPower>(choiceContext, play.Target, bleedPower, ownerCreature, this);
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
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}