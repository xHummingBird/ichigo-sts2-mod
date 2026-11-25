using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class Eclipse() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy
), IGetsugaCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(5m),
        new ExtraDamageVar(30m),

        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel card, Creature? _) =>
                {
                    var creature = card.Owner.Creature;

                    decimal hpLost = Math.Max(
                        0m,
                        creature.MaxHp - creature.CurrentHp);

                    var isInBankai =
                        creature.HasPower<TrueBankaiPower>() ||
                        creature.HasPower<BankaiHosPower>();

                    decimal bankaiMultiplier =
                        isInBankai ? 2m : 1m;

                    return
                        hpLost /
                        100m *
                        bankaiMultiplier;
                })
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

        if (Owner.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomPhrase();

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

            ichigo.PlayVfxOnTarget(
                target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit");

            await DamageCmd.Attack(
                    DynamicVars.CalculatedDamage)
                .FromCard(this, play)
                .Targeting(target)
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
            await DamageCmd.Attack(
                    DynamicVars.CalculatedDamage)
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
        DynamicVars.ExtraDamage.UpgradeValueBy(10m);
    }
}