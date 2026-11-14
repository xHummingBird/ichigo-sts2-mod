using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Extensions;
using Godot;

namespace Ichigo.IchigoCode.Relics;

[Pool(typeof(IchigoRelicPool))]
public abstract class IchigoRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".BigRelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
