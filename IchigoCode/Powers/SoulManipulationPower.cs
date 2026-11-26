using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;

namespace Ichigo.IchigoCode.Powers;

public sealed class SoulManipulationPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

	private bool _triggeredThisTurn;

	public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target, 
        DamageResult result, 
        ValueProp props, 
        Creature? dealer, 
        CardModel? cardSource)
	{
		if (Owner.CombatState != null && Owner.CombatState.CurrentSide == Owner.Side && target == Owner && result.UnblockedDamage > 0 && !_triggeredThisTurn)
		{
			_triggeredThisTurn = true;
			Flash();
			await PlayerCmd.GainEnergy(Amount, Owner.Player);
		}
	}

	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (!participants.Contains(Owner))
		{
			return Task.CompletedTask;
		}
		_triggeredThisTurn = false;
		return Task.CompletedTask;
	}

}
