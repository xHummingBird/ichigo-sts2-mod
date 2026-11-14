using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Common;
using Ichigo.IchigoCode.Character;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class Sonido() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self
)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new HpLossVar(2m), 
        new EnergyVar(1), 
        new BlockVar(15, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}