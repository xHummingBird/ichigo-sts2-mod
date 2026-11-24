using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class Deathblow() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move)
    ];

    protected override bool ShouldGlowGoldInternal =>
        CurrentTarget?.HasPower<BleedPower>() == true;

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

            await CommonActions.CardAttack(this, play)
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
            await CommonActions.CardAttack(this, play)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);
        }

        var bleedPower =
            target.GetPower<BleedPower>();

        if (bleedPower != null)
        {
            await bleedPower.Trigger();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}