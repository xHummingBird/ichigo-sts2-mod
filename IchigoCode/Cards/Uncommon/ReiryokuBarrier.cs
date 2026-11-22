using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards.Common;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Mechanics;


namespace Ichigo.IchigoCode.Cards.Uncommon;

public class ReiryokuBarrier() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Turns", 1),
        new BlockVar(5, ValueProp.Move)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
        HoverTipFactory.FromPower<SpiritualFracturePower>(),
        HoverTipFactory.FromPower<DefensePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DefensePower>(choiceContext, Owner.Creature, DynamicVars["Turns"].BaseValue, Owner.Creature, this);
        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
