using BaseLib.Utils;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Relics;

public sealed class SpiritCore : IchigoRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not SpiritualFracturePower)
            return;

        if (applier != Owner.Creature)
            return;

        if (amount <= 0)
            return;

        Flash();

        await CreatureCmd.GainBlock(Owner.Creature, 4, ValueProp.Unpowered, null);
    }
}