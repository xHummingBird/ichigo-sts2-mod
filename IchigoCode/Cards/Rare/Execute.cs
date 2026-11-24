using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class Execute() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(9m),
        new ExtraDamageVar(9m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel _, Creature? target) =>
                    target != null &&
                    target.CurrentHp * 2 < target.MaxHp
                        ? 1m
                        : 0m)
    ];

    protected override bool ShouldGlowGoldInternal =>
        CurrentTarget != null &&
        CurrentTarget.CurrentHp * 2 < CurrentTarget.MaxHp;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null &&
            Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomPhrase();

            await ichigo.DashTo(
                ownerCreature,
                play.Target!,
                distance: 300f,
                durationSeconds: 0.2f,
                overrideAnim: "dash");
            
            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");
            SfxCmd.Play(
                "res://Ichigo/sfx/slash_1.wav");
            await ichigo.DashTo
            (ownerCreature,
            play.Target!,
            distance: -300f,
            durationSeconds: 0.05f,
            overrideAnim: "execute");
            
            IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, play.Target!, hitSfx: "res://Ichigo/sfx/from_cloud/cloud_hit.wav", hitVfx: "execute_hit");
            await Task.Delay(200);
            IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, play.Target,
                hitSfx: "res://Ichigo/sfx/from_cloud/cloud_hit.wav", playHitVfx: false);
            await Task.Delay(67);
            IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, play.Target,
                hitSfx: "res://Ichigo/sfx/from_cloud/cloud_hit.wav", playHitVfx: false);
            await Task.Delay(67);
            await CommonActions.CardAttack(this, play)
                .WithHitFx(null, "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
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
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.ExtraDamage.UpgradeValueBy(3m);
    }
}