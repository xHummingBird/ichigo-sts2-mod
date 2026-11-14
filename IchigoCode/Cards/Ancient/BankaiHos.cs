using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Ancient;

public class BankaiHos() : IchigoCard(
    0,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Turns", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BankaiHosPower>(),
        HoverTipFactory.FromCard<GetsugaTenshou>()
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BankaiHosPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Turns"].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Turns"].UpgradeValueBy(1m);
    }
}