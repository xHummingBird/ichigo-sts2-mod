using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class FangSlash() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new HitCountVar(2)
    ];

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
            AudioHelper.PlayRandomAttack();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 250f,
                durationSeconds: 0.2f,
                overrideAnim: "attack");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing.wav");

            SfxCmd.Play(
                "res://Ichigo/sfx/slash_1.wav");

            await Task.Delay(50);

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars.HitCount.IntValue)
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

            await ichigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
        else
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars.HitCount.IntValue)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);
        }

        var randomAttack =
            PileType.Draw
                .GetPile(Owner)
                .Cards
                .Where(card => card.Type == CardType.Attack)
                .ToList()
                .StableShuffle(
                    Owner.RunState.Rng.CombatCardSelection)
                .FirstOrDefault();

        if (randomAttack == null)
            return;

        if (IsUpgraded && randomAttack.IsUpgradable)
        {
            CardCmd.Upgrade(randomAttack);
        }

        await CardCmd.AutoPlay(
            choiceContext,
            randomAttack,
            null);
    }
}