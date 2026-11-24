using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class Frenzy() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new HitCountVar(2)
    ];

    protected override bool ShouldGlowGoldInternal =>
        IsMutable &&
        Owner != null &&
        LostHpThisTurn(Owner.Creature);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null || target == null)
            return;

        var hitCount =
            LostHpThisTurn(ownerCreature)
                ? DynamicVars.HitCount.IntValue
                : 1;

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
                .WithHitCount(hitCount)
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
        DynamicVars.Damage.UpgradeValueBy(4m);
    }

    private static bool LostHpThisTurn(
        Creature creature)
    {
        return CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Any(entry =>
                entry.HappenedThisTurn(
                    creature.CombatState) &&
                entry.Receiver == creature &&
                entry.Result.UnblockedDamage > 0);
    }
}