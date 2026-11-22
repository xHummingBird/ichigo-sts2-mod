using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Common;

public class SubstituteShinigami() : IchigoCard(
    1,
    CardType.Power,
    CardRarity.Common,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Turns", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SubstituteShinigamiPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SubstituteShinigamiPower>(choiceContext, Owner.Creature, DynamicVars["Turns"].BaseValue, Owner.Creature, this);
    }
}
