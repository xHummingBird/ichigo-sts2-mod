using BaseLib.Utils;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Rare;

public class Berserk() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new HpLossVar(3m)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TrueBankaiPower>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!ReiryokuManager.IsInTrueBankai(Owner))
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        }
		foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards)
		{
			if (!card.EnergyCost.CostsX && card.Type == CardType.Attack)
			{
				card.SetToFreeThisTurn();
			}
		}
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
