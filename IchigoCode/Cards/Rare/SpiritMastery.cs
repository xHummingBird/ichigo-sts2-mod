using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Rare;

public class SpiritMastery() : IchigoCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self
)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SpiritualFracturePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SpiritMasteryPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
