using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class ArcSlash() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(
            play.Target,
            nameof(play.Target));

        var target = play.Target;
        var ownerCreature = Owner?.Creature;

        // Record this before attacking in case the attack defeats the enemy.
        var targetHasBleed =
            target.HasPower<BleedPower>();

        if (ownerCreature != null &&
            Owner?.Character is Character.Ichigo ichigo)
        {
            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 400f,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            ichigo.PlayAnimation(
                ownerCreature,
                "arc_slash");

            // First audio when the Arc Slash animation begins.
            SfxCmd.Play("res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            // 0.050s: second audio.
            await Task.Delay(50);

            SfxCmd.Play("res://Ichigo/sfx/slash_1.wav");

            // 0.100s: hit VFX, hit SFX, and real damage.
            await Task.Delay(50);

            ichigo.PlayVfxOnTarget(
                target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit");
        }
        await CommonActions.CardAttack(this, play)
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);

        if (targetHasBleed)
        {
            await PlayerCmd.GainEnergy(
                1,
                Owner!);
        }

        if (ownerCreature != null &&
            Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}