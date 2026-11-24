using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class BladeDance() : IchigoCard(
    3,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new HitCountVar(3)
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
            AudioHelper.PlayRandomPhrase();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 300f,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            SfxCmd.Play(
                "res://Ichigo/sfx/slash_1.wav");

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: -300f,
                durationSeconds: 0.05f,
                overrideAnim: "execute");

            await IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                target,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                hitVfx:
                    "execute_hit");

            await Task.Delay(200);

            await IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                target,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                playHitVfx: false);

            await Task.Delay(67);

            await IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                target,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                playHitVfx: false);

            await Task.Delay(67);

            await DamageCmd.Attack(
                    DynamicVars.Damage.BaseValue)
                .WithHitCount(
                    DynamicVars.HitCount.IntValue)
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
                .WithHitCount(
                    DynamicVars.HitCount.IntValue)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);
        }
    }

    public override Task AfterCardEnteredCombat(
        CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (IsClone)
            return Task.CompletedTask;

        int attacksPlayedThisTurn =
            CombatManager.Instance.History
                .CardPlaysFinished.Count(
                    (CardPlayFinishedEntry entry) =>
                        entry.CardPlay.Card.Type ==
                        CardType.Attack &&
                        entry.CardPlay.Player == Owner &&
                        entry.HappenedThisTurn(
                            CombatState));

        ReduceCostBy(
            attacksPlayedThisTurn);

        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        ReduceCostBy(1);

        return Task.CompletedTask;
    }

    private void ReduceCostBy(
        int amount)
    {
        EnergyCost.AddThisTurn(-amount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HitCount.UpgradeValueBy(1m);
    }
}