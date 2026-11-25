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

public class DualCrescent() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy
), IGetsugaCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new HitCountVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
    ];
    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.HasPower<TrueBankaiPower>() ||
        Owner.Creature.HasPower<BankaiHosPower>();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null || target == null)
            return;

        var isInBankai =
            ownerCreature.HasPower<TrueBankaiPower>() ||
            ownerCreature.HasPower<BankaiHosPower>();

        var hitCount = DynamicVars.HitCount.IntValue;

        if (isInBankai)
        {
            hitCount += 2;
        }

        if (Owner.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomAttack();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 250f,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            ichigo.PlayAnimation(
                ownerCreature,
                "rising_moon");

            await Task.Delay(50);

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing.wav");

            SfxCmd.Play(
                "res://Ichigo/sfx/slash_1.wav");

            await Task.Delay(50);

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hitCount)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitVfxNode(hitTarget =>
                {
                    ichigo.PlayVfxOnTarget(
                        hitTarget,
                        "res://Ichigo/scenes/vfx.tscn",
                        "hit");

                    return null;
                })
                .WithHitFx(null)
                .Execute(choiceContext);

            if (!target.HasPower<SpiritualFracturePower>())
            {
                await PowerCmd.Apply<SoulDisruptionPower>(
                    choiceContext,
                    target,
                    1m,
                    ownerCreature,
                    this);
            }

            await ichigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
        else
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hitCount)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);

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
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}