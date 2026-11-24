using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class EnhancedReflexes() : IchigoCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Unpowered)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EnhancedReflexesPower>(choiceContext, Owner.Creature, DynamicVars.Damage.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
