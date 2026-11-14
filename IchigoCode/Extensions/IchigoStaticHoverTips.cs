using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Ichigo.IchigoCode.Extensions;

public static class IchigoStaticHoverTips
{
    public static readonly IHoverTip Reiryoku =
        new HoverTip(
            new LocString(
                "static_hover_tips",
                "ICHIGO_REIRYOKU.title"),
            new LocString(
                "static_hover_tips",
                "ICHIGO_REIRYOKU.description"));
        
    public static readonly IHoverTip Getsuga =
        new HoverTip(
            new LocString(
                "static_hover_tips",
                "ICHIGO_GETSUGA.title"),
            new LocString(
                "static_hover_tips",
                "ICHIGO_GETSUGA.description"));
}