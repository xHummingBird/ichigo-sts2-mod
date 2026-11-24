using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Ichigo.IchigoCode.Cards.Rare;
using Ichigo.IchigoCode.Cards.Uncommon;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;

namespace Ichigo.IchigoCode.Powers;

public class SpiritMasteryPower :
    TemporaryStrengthPower,
    ICustomModel
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<SpiritMastery>();
}