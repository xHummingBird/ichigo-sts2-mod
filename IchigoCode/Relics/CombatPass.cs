using BaseLib.Utils;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ichigo.IchigoCode.Relics;

public sealed class CombatPass : IchigoRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SpiritualFracturePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SpiritualFracturePower>()
    ];

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

            await PowerCmd.Apply<SpiritualFracturePower>(
                choiceContext,
                combatState.HittableEnemies,
                DynamicVars["SpiritualFracturePower"].BaseValue,
                Owner.Creature,
                null);
        }
    }
}