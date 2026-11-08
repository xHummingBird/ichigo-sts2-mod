using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Character;

namespace Ichigo.IchigoCode.Cards.Basic;

public class DualZengetsu() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
		{
			CardModel token = CombatState!.CreateCard<GetsugaTenshou>(Owner);
			CardCmd.Upgrade(token);
			await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, Owner);
			await Cmd.Wait(0.25f);
		}
    }
}