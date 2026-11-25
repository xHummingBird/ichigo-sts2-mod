using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Ichigo.IchigoCode.Cards.Common;

public class SpiritSurge() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Common,
    TargetType.AnyEnemy
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new BlockVar(7, ValueProp.Move),
            new PowerVar<SpiritualFracturePower>(1)
        ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SpiritualFracturePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SpiritualFracturePower>(choiceContext, cardPlay.Target!, DynamicVars["SpiritualFracturePower"].BaseValue, Owner.Creature, this);
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["SpiritualFracturePower"].UpgradeValueBy(1m);
    }
}
