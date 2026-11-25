using BaseLib.Utils;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ichigo.IchigoCode.Relics;

public sealed class SennasRibbon : IchigoRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) &&
            Owner.PlayerCombatState.TurnNumber <= 1)
        {
            Flash();

            ReiryokuManager.GainReiryoku(Owner, 25);
        }
    }
}