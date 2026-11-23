using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class MoonFlash() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null || target == null)
            return;

        Dictionary<PowerModel, int> debuffAmounts =
            target.Powers
                .Where(power =>
                    power.TypeForCurrentAmount ==
                    PowerType.Debuff)
                .Select(power =>
                    (
                        Power:
                        (PowerModel)power.ClonePreservingMutability(),

                        Amount:
                        power.Amount
                    ))
                .ToDictionary(
                    entry => entry.Power,
                    entry => entry.Amount);

        foreach (var entry in debuffAmounts.ToList())
        {
            if (entry.Key is not ITemporaryPower temporaryPower)
                continue;

            var internallyAppliedEntry =
                debuffAmounts.FirstOrDefault(
                    candidate =>
                        candidate.Key.Id ==
                        temporaryPower.InternallyAppliedPower.Id);

            if (internallyAppliedEntry.Key != null)
            {
                debuffAmounts[internallyAppliedEntry.Key] +=
                    entry.Value;
            }
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

            ichigo.PlayVfxOnTarget(
                target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit");

            await CommonActions.CardAttack(this, play)
                .WithHitFx(null)
                .Execute(choiceContext);

            await ichigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
        else
        {
            await CommonActions.CardAttack(this, play)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);
        }

        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            if (enemy == target)
                continue;

            foreach (var entry in debuffAmounts)
            {
                if (entry.Value == 0)
                    continue;

                PowerModel? existingPower =
                    PowerCmd.FindExistingInstanceForStacking(
                        entry.Key,
                        enemy,
                        entry.Key.Applier);

                if (existingPower != null)
                {
                    await PowerCmd.ModifyAmount(
                        choiceContext,
                        existingPower,
                        entry.Value,
                        entry.Key.Applier,
                        this);

                    continue;
                }

                var copiedPower =
                    (PowerModel)entry.Key
                        .ClonePreservingMutability();

                await PowerCmd.Apply(
                    choiceContext,
                    copiedPower,
                    enemy,
                    entry.Value,
                    entry.Key.Applier,
                    this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}