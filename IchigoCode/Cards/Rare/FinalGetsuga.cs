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

public class FinalGetsuga() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Enemy
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(0)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		IEnumerable<CardModel> enumerable = PileType.Exhaust.GetPile(Owner).Cards.Where((CardModel c) => c is IGetsugaTenshouCard).ToList();
		bool flag = true;
		foreach (CardModel item in enumerable)
		{
			if (IsUpgraded)
			{
				CardCmd.Upgrade(item, CardPreviewStyle.None);
			}
			await CardCmd.AutoPlay(choiceContext, item, cardPlay.Target, AutoPlayType.Default, skipXCapture: false, !flag);
			flag = false;
		}
    }
}