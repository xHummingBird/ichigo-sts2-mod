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

public class ShunpoMastery() : IchigoCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Block", 2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ShunpoMasteryCounterPower>(choiceContext, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(1m);
    }
}
