using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class HeavenlyFlash() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const string CalculatedHitsKey =
        "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CalculationBaseVar(1m),
        new CalculationExtraVar(1m),

        new CalculatedHitCountVar(CalculatedHitsKey)
            .WithMultiplier(
                static (CardModel card, Creature? _) =>
                    CombatManager.Instance.History
                        .CardPlaysFinished.Count(
                            entry =>
                                entry.CardPlay.Player ==
                                card.Owner &&
                                entry.CardPlay.Card.Type ==
                                CardType.Skill &&
                                entry.HappenedThisTurn(
                                    card.CombatState)))
    ];

    protected override bool ShouldGlowGoldInternal =>
        CombatManager.Instance.History
            .CardPlaysFinished.Any(
                entry =>
                    entry.CardPlay.Player == Owner &&
                    entry.CardPlay.Card.Type ==
                    CardType.Skill &&
                    entry.HappenedThisTurn(CombatState));

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null || target == null)
            return;

        int hitCount =
            (int)((CalculatedVar)
                DynamicVars[CalculatedHitsKey])
                .Calculate(target);

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
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}