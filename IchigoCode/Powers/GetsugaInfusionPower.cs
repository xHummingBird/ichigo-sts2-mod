using BaseLib.Abstracts;
using BaseLib.Extensions;
using Ichigo.IchigoCode.Cards.Rare;
using Ichigo.IchigoCode.Cards.Uncommon;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Ichigo.IchigoCode.Powers;

public class GetsugaInfusionPower :
    TemporaryStrengthPower,
    ICustomPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<GetsugaInfusion>();

    public string? CustomPackedIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .BigPowerImagePath();

    public string? CustomBigIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .BigPowerImagePath();

    public string? CustomBigBetaIconPath =>
        null;
}