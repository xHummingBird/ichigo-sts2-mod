using MegaCrit.Sts2.Core.Entities.Relics;

namespace Ichigo.IchigoCode.Relics;

public class TensaZangetsu: IchigoRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override int ReiryokuPerTurn => 7;

    protected override int ReiryokuPerAttack => 5;

    protected override int HealPerCombatVictory => 7;
}
