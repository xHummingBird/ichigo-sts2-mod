using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Character;

namespace Ichigo.IchigoCode.Cards.Basic;

public class FlashStep() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new BlockVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
		{
            CardModel? getsuga =
                await GetsugaTenshou.CreateInHand(
                    Owner,
                    CombatState!,
                    Owner);
			await Cmd.Wait(0.25f);
		}
        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}