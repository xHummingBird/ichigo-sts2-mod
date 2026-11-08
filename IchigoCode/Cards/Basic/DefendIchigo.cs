using BaseLib.Utils;

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Ichigo.IchigoCode.Cards;

namespace Ichigo.IchigoCode.Cards.Basic;

public class DefendIchigo : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Basic,
    CardTarget.Self
)
{
    protected override HashSet<CardTag> canonicalTags => new HashSet<CardTag> { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommandActions.GainBlock(cardPlay, 5);
    }
}
