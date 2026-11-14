using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Ichigo.IchigoCode.Cards;
using Ichigo.IchigoCode.Powers;

namespace Ichigo.IchigoCode.Cards.Rare;

public class GetsugaInfusion() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self
)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GetsugaTenshouCounterPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}
