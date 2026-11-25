using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class NewMoon() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy),
    IGetsugaCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga,
        HoverTipFactory.FromCard<GetsugaTenshou>()
    ];

    protected override bool ShouldGlowGoldInternal =>
        IsInBankai;

    private bool IsInBankai =>
        IsMutable &&
        Owner != null &&
        (
            Owner.Creature.HasPower<TrueBankaiPower>() ||
            Owner.Creature.HasPower<BankaiHosPower>()
        );

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(
            play.Target,
            nameof(play.Target));

        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        if (ownerCreature != null &&
            Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomGetsuga();

            await ichigo.DashTo(
                ownerCreature,
                target,
                distance: 600f,
                durationSeconds: 0.1f,
                overrideAnim: "getsuga");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            SfxCmd.Play(
                "res://Ichigo/sfx/getsuga_3.wav",
                0.6f);

            await Task.Delay(216);

            ichigo.PlayVfxOnTarget(
                target,
                "res://Ichigo/scenes/vfx.tscn",
                "hit");
        }

        await CommonActions.CardAttack(this, play)
            .Targeting(target)
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);

        var getsugaCount =
            DynamicVars.Cards.IntValue +
            (IsInBankai ? 2 : 0);

        for (var i = 0; i < getsugaCount; i++)
        {
            CardModel? getsuga =
                await GetsugaTenshou.CreateInHand(
                    Owner!,
                    CombatState,
                    Owner);

            if (getsuga != null &&
                IsUpgraded &&
                getsuga.IsUpgradable)
            {
                CardCmd.Upgrade(getsuga);
            }

            await Cmd.Wait(0.25f);
        }

        if (ownerCreature != null &&
            Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.1f);
        }
    }

    protected override void OnUpgrade()
    {
    }
}