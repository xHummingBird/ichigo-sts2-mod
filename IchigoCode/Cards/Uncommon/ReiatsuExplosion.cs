using BaseLib.Extensions;
using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class ReiatsuExplosion() : IchigoCard(
    0,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new HitCountVar(0)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var enemies = CombatState.HittableEnemies.ToList();

        if (enemies.Count == 0)
            return;

        var middleEnemy = enemies[enemies.Count / 2];

        var hitCount =
            ResolveEnergyXValue() +
            DynamicVars.HitCount.IntValue;

        if (ReiryokuManager.GetReiryoku(Owner) >= 50)
            hitCount++;

        if (Owner?.Character is Character.Ichigo ichigo)
        {
            await ichigo.DashTo(
                ownerCreature,
                middleEnemy,
                distance: 200f,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            AudioHelper.PlayRandomPhrase();

            ichigo.PlayAnimation(
                ownerCreature,
                "explosion");

            // 0.333s: first audio.
            await Task.Delay(333);

            SfxCmd.Play("res://Ichigo/sfx/explosion.wav", 0.7f);

            // 0.400s: second audio.
            await Task.Delay(67);

            SfxCmd.Play("res://Ichigo/sfx/attack_effect_3.wav", 0.7f);

            // 0.433s: first fake hit.
            await Task.Delay(33);

            IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                middleEnemy,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                hitVfx:
                    "jujisho_hit");

            // 0.733s: second fake hit.
            await Task.Delay(300);

            IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                middleEnemy,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                hitVfx:
                    "jujisho_hit");

            // 1.033s: begin the real multi-hit attack.
            await Task.Delay(300);
        }

        await DamageCmd.Attack(
                DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
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
                        "jujisho_hit");
                }

                return null;
            })
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);
        await Task.Delay(750);

        if (Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HitCount.UpgradeValueBy(1m);
    }
}