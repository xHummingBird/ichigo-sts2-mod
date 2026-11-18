using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Common;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Mechanics;


namespace Ichigo.IchigoCode.Cards.Uncommon;

public class PowerNullification() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Turns", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PowerNullificationPower>(choiceContext, Owner.Creature, DynamicVars["Turns"].BaseValue, Owner.Creature, this);
    }
}

