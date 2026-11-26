using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Ichigo.IchigoCode.Powers;

public sealed class BattleInstinctPower : IchigoPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        IchigoStaticHoverTips.Getsuga
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;

        var ichigo =
            ModelDb.Character<Character.Ichigo>();

        IReadOnlyList<CardModel> getsugaPool =
            ichigo.CardPool
                .GetUnlockedCards(
                    player.UnlockState,
                    player.RunState.CardMultiplayerConstraint)
                .Where(card =>
                    card is IGetsugaCard &&
                    card.Rarity is not CardRarity.Basic &&
                    card.Rarity is not CardRarity.Ancient)
                .ToList();

        if (getsugaPool.Count == 0)
            return;

        CardModel[] generatedCards =
            new CardModel[Amount];

        Rng combatCardGeneration =
            player.RunState.Rng.CombatCardGeneration;

        for (int i = 0; i < Amount; i++)
        {
            CardModel getsuga =
                CardFactory.GetDistinctForCombat(
                    player,
                    getsugaPool,
                    1,
                    combatCardGeneration)
                .First();

            CardCmd.ApplyKeyword(
                getsuga,
                CardKeyword.Exhaust);

            generatedCards[i] = getsuga;
        }

        Flash();

        await CardPileCmd.AddGeneratedCardsToCombat(
            generatedCards,
            PileType.Hand,
            player);
    }
}