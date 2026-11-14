using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Cards.Basic;

public class DualZengetsu() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await GetsugaTenshouTokens.AddToHand(CombatState!, Owner, DynamicVars.Cards.IntValue, upgrade: true);
    }
}