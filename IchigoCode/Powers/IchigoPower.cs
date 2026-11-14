using BaseLib.Abstracts;
using BaseLib.Extensions;
using Ichigo.IchigoCode.Extensions;
using Godot;

namespace Ichigo.IchigoCode.Powers;

public abstract class IchigoPower : CustomPowerModel
{
    //Loads from Ichigo/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
