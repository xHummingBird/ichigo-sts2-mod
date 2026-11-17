using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Cards.Ancient;

public class TrueZangetsu() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            CardModel? getsuga =
                await GetsugaTenshou.CreateInHand(
                    Owner,
                    CombatState!,
                    Owner);

            if (getsuga != null && IsUpgraded)
                CardCmd.Upgrade(getsuga);

            await Cmd.Wait(0.25f);
        }
		foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards)
		{
			if (!card.EnergyCost.CostsX && card is IGetsugaCard)
			{
                card.EnergyCost.SetThisCombat(0, reduceOnly: true);
			}
		}
    }
}