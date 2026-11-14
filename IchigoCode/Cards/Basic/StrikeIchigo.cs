using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Basic;

public class StrikeIchigo() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Basic,
    TargetType.AnyEnemy
)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomAttack();
            await ichigo.DashTo(ownerCreature, play.Target, distance: 250f, durationSeconds: 0.2F, overrideAnim: "attack");
            SfxCmd.Play("res://Ichigo/sfx/from_cloud/sword_swing.wav");
            SfxCmd.Play("res://Ichigo/sfx/slash_1.wav");
            await Task.Delay(50);
            ichigo.PlayVfxOnTarget(
                play.Target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit"
            );
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null)
                .Execute(choiceContext);
            await ichigo.Retreat(ownerCreature, duration: 0.2F);
        }
        else  await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Ichigo/sfx/slash_1.wav")
            .Execute(choiceContext);
        
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}