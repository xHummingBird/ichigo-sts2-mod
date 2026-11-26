using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class ReiatsuControl() : IchigoCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self
)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Reiryoku", 3),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ReiatsuControlPower>(choiceContext, Owner.Creature, DynamicVars["Reiryoku"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
