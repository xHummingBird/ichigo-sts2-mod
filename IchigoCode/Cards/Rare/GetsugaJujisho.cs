using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;


public class GetsugaJujisho() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies), IGetsugaCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
        HoverTipFactory.FromCard<GetsugaTenshou>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(7m),
        new ExtraDamageVar(2m),

        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(
                static (CardModel card, Creature? _) =>
                {
                    var getsugaCount =
                        CombatManager.Instance.History.CardPlaysFinished.Count(
                            entry =>
                                entry.CardPlay.Player == card.Owner &&
                                entry.CardPlay.Card is IGetsugaCard);

                    var isInBankai =
                        card.Owner.Creature.HasPower<TrueBankaiPower>() ||
                        card.Owner.Creature.HasPower<BankaiHosPower>();

                    return isInBankai
                        ? getsugaCount * 2
                        : getsugaCount;
                })
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
            var ownerCreature = Owner?.Creature;
            var firstEnemy = CombatState.HittableEnemies.FirstOrDefault();
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            if (ownerCreature != null &&
                firstEnemy != null &&
                Owner?.Character is Character.Ichigo ichigo)
            {
                
                if (ownerCreature.HasPower<TrueBankaiPower>())
                    ichigo.PlayCutIn("jujisho_bankai");
                else if (ownerCreature.HasPower<BankaiHosPower>())
                    ichigo.PlayCutIn("jujisho_hos");
                else ichigo.PlayCutIn("jujisho_base");
                SfxCmd.Play("res://Ichigo/sfx/cutin_sfx.wav");
                await Task.Delay(800);
                ichigo.PlayAnimation(ownerCreature, "jujisho");
                SfxCmd.Play("res://Ichigo/sounds/getsuga_jujisho.wav", 0.7f);
                SfxCmd.Play("res://Ichigo/sfx/attack_effect_2.wav", 0.7f);
                await Task.Delay(400);
                SfxCmd.Play("res://Ichigo/sfx/getsuga_4.wav", 0.7f);
                await Task.Delay(200);
                foreach (var enemy in CombatState.HittableEnemies)
                    ichigo.PlayVfxOnTarget(
                        enemy,
                        "res://Ichigo/scenes/vfx.tscn",
                        "jujisho_hit"
                    );
            }
            await CommonActions.CardAttack(this, play)
                .WithHitFx(null, "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
                .Execute(choiceContext);
            await Task.Delay(200);
            foreach (var enemy in CombatState.HittableEnemies)
            {
                if (!enemy.HasPower<SpiritualFracturePower>())
                    await PowerCmd.Apply<SoulDisruptionPower>(choiceContext, enemy, 1, Owner!.Creature, this);
            }
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
    
}