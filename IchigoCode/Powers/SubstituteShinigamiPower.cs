using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;

namespace Ichigo.IchigoCode.Powers;

public sealed class SubstituteShinigamiPower : IchigoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new EnergyVar(2),
        ]; 

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.EnergyCost.GetResolved() >= DynamicVars.Energy.IntValue)
        {
            Flash();
            await GetsugaTenshou.CreateInHand(
                Owner.Player!,
                CombatState!,
                Owner.Player);
        }
    }
}
