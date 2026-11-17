using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class PowerOfVisored() : IchigoCard(
    0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3),
        new CardsVar(3),
        new DynamicVar("Reiryoku", 10)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable |
            ValueProp.Unpowered |
            ValueProp.Move,
            this,
            cardPlay);

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            await GetsugaTenshou.CreateInHand(
                Owner,
                CombatState!,
                Owner);

            await Cmd.Wait(0.25f);
        }

        ReiryokuManager.GainReiryoku(
            Owner,
            DynamicVars["Reiryoku"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HpLoss.UpgradeValueBy(-1m);
        DynamicVars["Reiryoku"].UpgradeValueBy(5m);
    }
}
