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
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Rare;

public class FinalGetsuga() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.AnyEnemy
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(0)];
    
    public override TargetType TargetType =>
	    IsInBankai
		    ? TargetType.AllEnemies
		    : TargetType.AnyEnemy;
    
    private bool IsInTrueBankai =>
	    IsMutable &&
	    Owner != null &&
	    Owner.Creature.HasPower<TrueBankaiPower>();

    private bool IsInHosBankai =>
	    IsMutable &&
	    Owner != null &&
	    Owner.Creature.HasPower<BankaiHosPower>();

    private bool IsInBankai =>
	    IsInTrueBankai ||
	    IsInHosBankai;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		IEnumerable<CardModel> enumerable = PileType.Exhaust.GetPile(Owner).Cards.Where((CardModel c) => c is GetsugaTenshou).ToList();
		bool flag = true;
		foreach (CardModel item in enumerable)
		{
			if (IsUpgraded)
			{
				CardCmd.Upgrade(item, CardPreviewStyle.None);
			}
			if (IsInBankai)
				await CardCmd.AutoPlay(choiceContext, item, null, AutoPlayType.Default, skipXCapture: false, !flag);
			else
				await CardCmd.AutoPlay(choiceContext, item, cardPlay.Target, AutoPlayType.Default, skipXCapture: false, !flag);
			flag = false;
		}
    }
}
