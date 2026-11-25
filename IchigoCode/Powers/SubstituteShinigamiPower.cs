using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ichigo.IchigoCode.Powers;

public sealed class SubstituteShinigamiPower : IchigoPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2)
    ];

    public override async Task BeforeCardPlayed(
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        if (cardPlay.Card.EnergyCost.GetResolved() <
            DynamicVars.Energy.IntValue)
        {
            return;
        }

        Flash();

        for (int i = 0; i < Amount; i++)
        {
            await GetsugaTenshou.CreateInHand(
                Owner.Player!,
                CombatState!,
                Owner.Player);

            await Cmd.Wait(0.25f);
        }
    }
}