using BaseLib.Utils;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Ichigo.IchigoCode.Cards.Rare;

public class BattleInstinct() : IchigoCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BattleInstinctPower>(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IchigoStaticHoverTips.Getsuga
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BattleInstinctPower>(choiceContext, Owner.Creature, DynamicVars["BattleInstinctPower"].BaseValue, Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
