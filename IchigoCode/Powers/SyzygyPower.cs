using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

// Lasts the entire battle.
public sealed class SyzygyPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner.Player)
		{
            Flash();
            await GetsugaTenshou.CreateInHand(
                Owner.Player!,
                CombatState!,
                Owner.Player);
		}
	}
}
