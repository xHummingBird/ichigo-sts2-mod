using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class CeroCreciente() : IchigoCard(
    2,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AllEnemies), IGetsugaCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3),
        new DamageVar(22m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga
    ];

    private bool IsInBankai =>
        IsMutable &&
        Owner != null &&
        (
            Owner.Creature.HasPower<TrueBankaiPower>() ||
            Owner.Creature.HasPower<BankaiHosPower>()
        );

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await LoseHpOrHeal(choiceContext, cardPlay);

        var ownerCreature = Owner?.Creature;
        var firstEnemy = CombatState.HittableEnemies.FirstOrDefault();

        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null &&
            firstEnemy != null &&
            Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomPhrase();

            await ichigo.DashTo(
                ownerCreature,
                firstEnemy,
                distance: 400f,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            ichigo.PlayAnimation(ownerCreature, "cero");
            // 0.000s - First audio
            SfxCmd.Play("res://Ichigo/sfx/from_cloud/limit_break_thunder.wav");

            // 0.250s - Second audio
            await Task.Delay(250);

            SfxCmd.Play("res://Ichigo/sfx/attack_effect_2.wav", 0.7f);

            // 0.267s - Third audio
            await Task.Delay(17);

            SfxCmd.Play("res://Ichigo/sfx/attack_effect_1.wav", 0.7f);

            // 0.300s - First fake hit
            // 0.300 - 0.267 = 0.033 seconds
            await Task.Delay(33);

            IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                firstEnemy,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                hitVfx:
                    "hit");

            // 0.500s - Second fake hit
            await Task.Delay(200);

            IchigoExtensions.CombatHelpers.IchigoFakeHit(
                ownerCreature,
                firstEnemy,
                hitSfx:
                    "res://Ichigo/sfx/from_cloud/cloud_hit.wav",
                hitVfx:
                    "hit");

            // 0.700s - Real hit
            await Task.Delay(200);
        }

        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);

        if (IsInBankai)
        {
                await PowerCmd.Apply<SpiritualFracturePower>(
                    choiceContext,
                    CombatState.HittableEnemies,
                    2,
                    Owner!.Creature,
                    this);

                await PowerCmd.Apply<BleedPower>(
                    choiceContext,
                    CombatState.HittableEnemies,
                    5,
                    Owner.Creature,
                    this);
        }
        
        else foreach (var enemy in CombatState.HittableEnemies)
            if (!enemy.HasPower<SpiritualFracturePower>())
                await PowerCmd.Apply<SoulDisruptionPower>(
                    choiceContext,
                    enemy,
                    1,
                    Owner!.Creature,
                    this);

        if (ownerCreature != null &&
            Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}