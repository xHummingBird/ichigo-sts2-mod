using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Common;

public class HollowGuard() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new HpLossVar(1m),
        new BlockVar(10m, ValueProp.Move)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await LoseHpOrHeal(choiceContext, cardPlay);
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
