using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Cards.Common;

public class SpiritualAwareness() : IchigoCard(
    0,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new BlockVar(7, ValueProp.Move), 
            new DynamicVar("Reiryoku", 7)
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
        ReiryokuManager.GainReiryoku(Owner, DynamicVars["Reiryoku"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Reiryoku"].UpgradeValueBy(3m);
    }
}
