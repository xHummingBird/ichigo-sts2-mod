using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class FinalBlow() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new ExtraDamageVar(1m),

        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel card, Creature? _) =>
                    CombatManager.Instance.History.CardPlaysFinished.Count(
                        entry =>
                            entry.CardPlay.Player == card.Owner))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null || target == null)
            return;

        var isTrueBankai =
            ownerCreature.HasPower<TrueBankaiPower>();
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (Owner.Character is Character.Ichigo ichigo)
        {
            if (isTrueBankai)
            {
                SfxCmd.Play("res://Ichigo/sounds/final_blow.mp3", 0.7f);
                ichigo.PlayCutIn("jujisho_bankai");
                SfxCmd.Play("res://Ichigo/sfx/cutin_sfx.wav");
                await Task.Delay(1000);
                
                await ichigo.DashTo(
                    ownerCreature,
                    target,
                    distance: 300f,
                    durationSeconds: 0.2f,
                    overrideAnim: "dash");
                ichigo.PlayAnimation(ownerCreature, "final_blow");
                SfxCmd.Play("res://Ichigo/sfx/attack_effect_2.wav", 0.7f);
                await Task.Delay(300);
                SfxCmd.Play("res://Ichigo/sfx/attack_effect_3.wav", 0.7f);
                await Task.Delay(16);
                SfxCmd.Play("res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");
                await Task.Delay(17);
                IchigoExtensions.CombatHelpers.IchigoFakeHit(
                    ownerCreature,
                    target,
                    hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                    hitVfx:
                    "execute_hit");

                await Task.Delay(200);

                IchigoExtensions.CombatHelpers.IchigoFakeHit(
                    ownerCreature,
                    target,
                    hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                    playHitVfx: false);

                await Task.Delay(67);

                IchigoExtensions.CombatHelpers.IchigoFakeHit(
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

                IchigoExtensions.CombatHelpers.IchigoFakeHit(
                    ownerCreature,
                    target,
                    hitSfx:
                        "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                    hitVfx:
                        "execute_hit");

                await Task.Delay(200);

                IchigoExtensions.CombatHelpers.IchigoFakeHit(
                    ownerCreature,
                    target,
                    hitSfx:
                        "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                    playHitVfx: false);

                await Task.Delay(67);

                IchigoExtensions.CombatHelpers.IchigoFakeHit(
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
        }
        else
        {
            await CommonActions.CardAttack(this, play)
                .WithHitFx(
                    null,
                    "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}