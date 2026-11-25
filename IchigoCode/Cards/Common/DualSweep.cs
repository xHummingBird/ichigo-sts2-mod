using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class DualSweep() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new HitCountVar(2),
        new CardsVar(1)
    ];

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
        }

        await DamageCmd.Attack(
                DynamicVars.Damage.BaseValue)
            .WithHitCount(
                DynamicVars.HitCount.IntValue)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithNoAttackerAnim()
            .WithHitVfxNode(target =>
            {
                if (Owner?.Character is Character.Ichigo ichigo)
                {
                    ichigo.PlayVfxOnTarget(
                        target,
                        "res://Ichigo/scenes/vfx.tscn",
                        "hit");
                }

                return null;
            })
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.IntValue,
            Owner!);

        if (Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}