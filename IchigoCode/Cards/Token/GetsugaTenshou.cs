using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Token;

public class GetsugaTenshou() : IchigoCard(
    0,
    CardType.Attack,
    CardRarity.Token,
    TargetType.AnyEnemy), IGetsugaTenshouCard
{
    public override TargetType TargetType =>
        IsInBankai
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
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
        AttackCommand attackCommand =
            DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay);

        if (IsInBankai)
        {
            attackCommand =
                attackCommand.TargetingAllOpponents(
                    CombatState);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(
                cardPlay.Target,
                nameof(cardPlay.Target));
            
            var ownerCreature = Owner?.Creature;

            if (ownerCreature != null && Owner?.Character is Character.Ichigo ichigo)
            {
                AudioHelper.PlayRandomAttack();
                await ichigo.DashTo(ownerCreature, cardPlay.Target, distance: 600f, durationSeconds: 0.1F,
                    overrideAnim: "getsuga");
                SfxCmd.Play("res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");
                SfxCmd.Play("res://Ichigo/sfx/getsuga_3.wav", 0.8f);
                await Task.Delay(216);
                ichigo.PlayVfxOnTarget(
                    cardPlay.Target,
                    "res://Ichigo/scenes/vfx.tscn",
                    "hit"
                );
                await attackCommand.Targeting(
                        cardPlay.Target)
                    .WithHitFx(null, "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
                    .Execute(choiceContext);
                await ichigo.Retreat(ownerCreature, duration: 0.1F);
            }
            else await attackCommand.Execute(
                choiceContext);
            await PowerCmd.Apply<SpiritualFracturePower>(choiceContext, cardPlay.Target, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    public static async Task<CardModel?> CreateInHand(
        Player owner,
        ICombatState combatState,
        Player? creator = null)
    {
        return (
            await CreateInHand(
                owner,
                1,
                combatState,
                creator))
            .FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState,
        Player? creator = null)
    {
        if (count == 0)
            return Array.Empty<CardModel>();

        if (CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();

        List<CardModel> getsugas = [];

        for (int i = 0; i < count; i++)
        {
            getsugas.Add(
                combatState.CreateCard<GetsugaTenshou>(
                    owner));
        }

        await CardPileCmd.AddGeneratedCardsToCombat(
            getsugas,
            PileType.Hand,
            creator ?? owner);

        return getsugas;
    }
}