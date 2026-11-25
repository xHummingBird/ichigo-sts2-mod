using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class RisingMoon() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy
), IGetsugaCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomAttack();
            await ichigo.DashTo(ownerCreature, play.Target!, distance: 250f, durationSeconds: 0.2F, overrideAnim: "dash");
            ichigo.PlayAnimation(ownerCreature, "rising_moon");
            await Task.Delay(50);
            SfxCmd.Play("res://Ichigo/sfx/from_cloud/sword_swing.wav");
            SfxCmd.Play("res://Ichigo/sfx/slash_1.wav");
            await Task.Delay(50);
            ichigo.PlayVfxOnTarget(
                play.Target!,
                "res://Ichigo/scenes/vfx.tscn",
                "hit"
            );
            await CommonActions.CardAttack(this, play)
                .WithHitFx(null)
                .Execute(choiceContext);
            if (!play.Target.HasPower<SpiritualFracturePower>())
                await PowerCmd.Apply<SoulDisruptionPower>(choiceContext, play.Target, 1, Owner!.Creature, this); 
            await ichigo.Retreat(ownerCreature, duration: 0.2F);
        }
        else
        {
            await CommonActions.CardAttack(this, play)
                .WithHitFx(null, "res://Ichigo/sfx/slash_1.wav")
                .Execute(choiceContext);
            if (!play.Target.HasPower<SpiritualFracturePower>())
                await PowerCmd.Apply<SoulDisruptionPower>(choiceContext, play.Target, 1, Owner!.Creature, this); 
        }
        if (ownerCreature.HasPower<TrueBankaiPower>() || ownerCreature.HasPower<BankaiHosPower>())
            await PowerCmd.Apply<FreeSkillPower>(choiceContext, Owner.Creature, 1, Owner!.Creature, this); 
        await PowerCmd.Apply<FreeAttackPower>(choiceContext, Owner.Creature, 1, Owner!.Creature, this); 
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}