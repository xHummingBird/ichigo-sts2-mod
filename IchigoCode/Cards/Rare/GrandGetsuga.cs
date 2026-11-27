using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Rare;

public class GrandGetsuga() : IchigoCard(
    8,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy),
    IGetsugaCard
{
    public override TargetType TargetType =>
        IsInBankai
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga
    ];

    protected override bool ShouldGlowGoldInternal =>
        IsInBankai;

    private bool IsInTrueBankai =>
        IsMutable &&
        Owner != null &&
        Owner.Creature.HasPower<TrueBankaiPower>();

    private bool IsInHosBankai =>
        IsMutable &&
        Owner != null &&
        Owner.Creature.HasPower<BankaiHosPower>();

    private bool IsInBankai =>
        IsInTrueBankai ||
        IsInHosBankai;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature == null)
            return;

        var referenceTarget = IsInBankai
            ? CombatState.HittableEnemies.FirstOrDefault()
            : play.Target;

        if (referenceTarget == null)
            return;

        if (Owner?.Character is Character.Ichigo ichigo)
        {
            // Getsuga Tenshou phrase before the cut-in.
            if (IchigoConfig.EnableVoice)
                SfxCmd.Play("res://Ichigo/sounds/getsuga_final.mp3", 0.7f);

            var cutInName = IsInTrueBankai
                ? "jujisho_bankai"
                : IsInHosBankai
                    ? "jujisho_hos"
                    : "jujisho_base";

            ichigo.PlayCutIn(cutInName);

            SfxCmd.Play(
                "res://Ichigo/sfx/cutin_sfx.wav");

            await Task.Delay(800);

            await ichigo.DashTo(
                ownerCreature,
                referenceTarget,
                distance: 300f,
                durationSeconds: 0.2f,
                overrideAnim: "grand_getsuga");

            // All three audio files begin together when Ichigo
            // reaches the reference enemy.
            SfxCmd.Play("res://Ichigo/sfx/getsuga_7.wav", 0.7f);
            SfxCmd.Play("res://Ichigo/sfx/attack_effect_1.wav", 0.7f);
            SfxCmd.Play("res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            if (IsInBankai)
            {
                SfxCmd.Play("res://Ichigo/sfx/from_cloud/cloud_hit.wav");
                foreach (var enemy in CombatState.HittableEnemies.ToList())
                    IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, enemy, hitVfx: "execute_hit");

                await Task.Delay(200);
                SfxCmd.Play("res://Ichigo/sfx/from_cloud/cloud_hit.wav");
                foreach (var enemy in CombatState.HittableEnemies.ToList())
                    IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, enemy, playHitVfx: false);

                await Task.Delay(67);
                SfxCmd.Play("res://Ichigo/sfx/from_cloud/cloud_hit.wav");
                foreach (var enemy in CombatState.HittableEnemies.ToList())
                    IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, enemy, playHitVfx: false);
            }
            else
            {
                IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, referenceTarget, hitSfx: "res://Ichigo/sfx/from_cloud/cloud_hit.wav", hitVfx: "execute_hit");

                await Task.Delay(200);

                IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, referenceTarget, hitSfx: "res://Ichigo/sfx/from_cloud/cloud_hit.wav", playHitVfx: false);

                await Task.Delay(67);

                IchigoExtensions.CombatHelpers.IchigoFakeHit(ownerCreature, referenceTarget, hitSfx: "res://Ichigo/sfx/from_cloud/cloud_hit.wav", playHitVfx: false);
            }
            await Task.Delay(67);
        }
        CommonActions.CardAttack(this, play)
            .WithHitFx(null, "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);
        
        if (IsInBankai)
            foreach (var enemy in CombatState.HittableEnemies)
                if (!enemy.HasPower<SpiritualFracturePower>())
                    await PowerCmd.Apply<SoulDisruptionPower>(choiceContext, enemy, 1, Owner!.Creature, this);
                else 
                if (!play.Target.HasPower<SpiritualFracturePower>())
                    await PowerCmd.Apply<SoulDisruptionPower>(choiceContext, play.Target, 1, Owner!.Creature, this);
        
        
        
        if (Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
    }

    public override Task AfterCardEnteredCombat(
        CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (IsClone)
            return Task.CompletedTask;

        var getsugaCardsPlayed =
            CombatManager.Instance.History.CardPlaysFinished.Count(
                (CardPlayFinishedEntry entry) =>
                    entry.CardPlay.Player == Owner &&
                    entry.CardPlay.Card is IGetsugaCard);

        EnergyCost.AddThisCombat(
            -getsugaCardsPlayed);

        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card is not IGetsugaCard)
            return Task.CompletedTask;

        EnergyCost.AddThisCombat(-1);

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}