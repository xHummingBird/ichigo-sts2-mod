using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class PointBlankGetsuga() : IchigoCard(
    1,
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

    protected override bool ShouldGlowGoldInternal =>
        IsInBankai;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<BleedPower>(3m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
        HoverTipFactory.FromPower<BleedPower>()
    ];


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

        decimal bleedAmount =
            DynamicVars["BleedPower"].BaseValue;

        if (isInBankai)
        {
            bleedAmount *= 2m;
        }

        await PowerCmd.Apply<BleedPower>(
            choiceContext,
            target,
            bleedAmount,
            ownerCreature,
            this);

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
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["BleedPower"].UpgradeValueBy(1m);
    }
}