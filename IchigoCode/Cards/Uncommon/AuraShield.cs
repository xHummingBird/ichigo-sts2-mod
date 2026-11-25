using BaseLib.Utils;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Uncommon;

public class AuraShield() : IchigoCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<GetsugaTenshou>(),
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("GetsugaExhaustMultiplier", 2m),
        ..MakeCalculatedBlock(4, (CardModel card, Creature? _) =>
        {
            if (card.Owner == null)
            {
                return 0m;
            }

            int count = PileType.Exhaust.GetPile(card.Owner).Cards.Count(c => c is IGetsugaTenshouCard);
            return count * card.DynamicVars["GetsugaExhaustMultiplier"].BaseValue;
        }, 1, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, DynamicVars.CalculatedBlock, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GetsugaExhaustMultiplier"].UpgradeValueBy(1m);
    }
}
