using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Ichigo.IchigoCode.Cards.Basic;

public class DualZangetsu() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
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

            if (getsuga != null & IsUpgraded)
                CardCmd.Upgrade(getsuga);

            await Cmd.Wait(0.25f);
        }
    }
}