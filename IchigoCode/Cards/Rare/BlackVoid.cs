using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class BlackVoid() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13m, ValueProp.Move),
        new HpLossVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(
            play.Target,
            nameof(play.Target));

        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature == null)
            return;

        await LoseHpOrHeal(
            choiceContext,
            play);

        await CreatureCmd.LoseBlock(
            choiceContext,
            target,
            target.Block,
            ownerCreature);

        if (Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomAttack();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 550f,
                durationSeconds: 0.2f,
                overrideAnim: "black_getsuga");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            await Task.Delay(33);

            SfxCmd.Play(
                "res://Ichigo/sfx/getsuga_6.wav",
                0.8f);

            await Task.Delay(17);

            ichigo.PlayVfxOnTarget(
                target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit_black");

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
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        AddKeyword(CardKeyword.Retain);
    }
}