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

public class GetsugaAura() : IchigoCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self
)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AdditionalDamage", 3),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GetsugaAuraPower>(choiceContext, Owner.Creature, DynamicVars["AdditionalDamage"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AdditionalDamage"].UpgradeValueBy(1m);
    }
}
