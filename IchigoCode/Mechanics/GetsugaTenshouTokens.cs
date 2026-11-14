using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;

namespace Ichigo.IchigoCode.Mechanics;

public static class GetsugaTenshouTokens
{
    public static async Task AddToHand(ICombatState combatState, Player owner, int count, bool upgrade = false)
    {
        for (int i = 0; i < count; i++)
        {
            CardModel token = combatState.CreateCard<GetsugaTenshou>(owner);
            if (upgrade)
            {
                CardCmd.Upgrade(token);
            }
            await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, owner);
            await Cmd.Wait(0.25f);
        }
    }
}
