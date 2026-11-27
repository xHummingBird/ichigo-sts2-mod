using BaseLib.Config;

namespace Ichigo.IchigoCode.Extensions;

public sealed class IchigoConfig : SimpleModConfig
{
    [ConfigSection("Audio")]
    public static bool EnableCustomBattleMusic { get; set; } = true;

    public static bool EnableCustomVictoryMusic { get; set; } = true;

    public static bool EnableBankaiMusic { get; set; } = true;
}
