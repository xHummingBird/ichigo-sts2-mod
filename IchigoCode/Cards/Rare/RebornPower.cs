using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Rare;

public class RebornPower() : IchigoCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self
)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
        HoverTipFactory.FromCard<GetsugaTenshou>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BleedPower>(2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RebornPowerPower>(choiceContext, Owner.Creature, DynamicVars["BleedPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BleedPower"].UpgradeValueBy(1);
    }
}
