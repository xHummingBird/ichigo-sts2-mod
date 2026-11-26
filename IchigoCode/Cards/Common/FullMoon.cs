using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class FullMoon() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies),
    IGetsugaCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(6m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel card, Creature? target) =>
                {
                    if (card.Owner == null)
                        return 0m;

                    var isInBankai =
                        card.Owner.Creature.HasPower<TrueBankaiPower>() ||
                        card.Owner.Creature.HasPower<BankaiHosPower>();

                    return isInBankai &&
                           card.CombatState.HittableEnemies.Count() == 1
                        ? 1m
                        : 0m;
                })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga
    ];

    private bool IsInBankai =>
        IsMutable &&
        Owner != null &&
        (
            Owner.Creature.HasPower<TrueBankaiPower>() ||
            Owner.Creature.HasPower<BankaiHosPower>()
        );

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var enemies = CombatState.HittableEnemies.ToList();

        if (ownerCreature == null ||
            enemies.Count == 0)
        {
            return;
        }

        var firstEnemy = enemies[0];

        var dashDistance = enemies.Count > 1
            ? 150f
            : 400f;

        if (Owner?.Character is Character.Ichigo ichigo)
        {
            await ichigo.DashTo(
                ownerCreature,
                firstEnemy,
                distance: dashDistance,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            ichigo.PlayAnimation(
                ownerCreature,
                "arc_slash");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            await Task.Delay(50);

            SfxCmd.Play(
                "res://Ichigo/sfx/slash_1.wav");

            await Task.Delay(50);

            foreach (var enemy in enemies)
            {
                ichigo.PlayVfxOnTarget(
                    enemy,
                    "res://Ichigo/scenes/vfx.tscn",
                    "hit");
            }
        }

        var damage = DynamicVars.Damage.BaseValue;

        if (IsInBankai &&
            enemies.Count == 1)
        {
            damage *=
                1m +
                DynamicVars.ExtraDamage.BaseValue / 100m;
        }

        await DamageCmd.Attack(damage)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithNoAttackerAnim()
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);
        
        if (!play.Target.HasPower<SpiritualFracturePower>())
            await PowerCmd.Apply<SoulDisruptionPower>(choiceContext, play.Target, 1, Owner!.Creature, this);

        if (Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4m);
        DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}