using BaseLib.Utils;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Cards.Rare;

public class PowerFusion() : IchigoCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self
)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> enumerable = PileType.Hand.GetPile(Owner).Cards.ToList();
        int handSize = enumerable.Count();
        await CardCmd.Discard(choiceContext, enumerable);
		await Cmd.CustomScaledWait(0f, 0.25f);
		await GetsugaTenshouTokens.AddToHand(CombatState!, Owner, handSize);
    }
}