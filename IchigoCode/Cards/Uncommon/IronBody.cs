using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Common;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Mechanics;


namespace Ichigo.IchigoCode.Cards.Uncommon;

public class IronBody() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self
)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(20, ValueProp.Move),
        new DynamicVar("Reiryoku", 20)
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
        ReiryokuManager.GainReiryoku(Owner, DynamicVars["Reiryoku"].IntValue);
    }

    protected override void OnUpgrade(){
        DynamicVars.Block.UpgradeValueBy(8m);
        DynamicVars["Reiryoku"].UpgradeValueBy(15m);
    }
}