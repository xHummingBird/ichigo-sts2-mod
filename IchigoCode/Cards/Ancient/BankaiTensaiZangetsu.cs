using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Cards.Token;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Cards.Ancient;

public class BankaiTensaiZangetsu() : IchigoCard(
    0,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new DynamicVar("Reiryoku", 100)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            await GetsugaTenshou.CreateInHand(
                Owner,
                CombatState!,
                Owner);

            await Cmd.Wait(0.25f);
        }
        bool isBankai =
            Owner.Creature.HasPower<TrueBankaiPower>() ||
            Owner.Creature.HasPower<BankaiHosPower>();

        if (isBankai)
        {
            if (Owner.Creature.HasPower<TrueBankaiPower>())
                await PowerCmd.Apply<TrueBankaiPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this);
            else
                await PowerCmd.Apply<BankaiHosPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this);
        } else {
            ReiryokuManager.GainReiryoku(Owner, DynamicVars["Reiryoku"].IntValue);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
