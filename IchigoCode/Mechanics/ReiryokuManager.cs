using MegaCrit.Sts2.Core.Entities.Players;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Relics;

namespace Ichigo.IchigoCode.Mechanics;

public static class ReiryokuManager
{
    private static IchigoRelicBase? FindIchigoRelic(Player player)
    {
        return player.Relics
            .OfType<IchigoRelicBase>()
            .FirstOrDefault();
    }

    public static bool IsInTrueBankai(Player player)
    {
        return player.Creature.HasPower<TrueBankaiPower>();
    }

    public static bool IsInBankaiHos(Player player)
    {
        return player.Creature.HasPower<BankaiHosPower>();
    }

    public static bool CanGainReiryoku(Player player)
    {
        return !IsInTrueBankai(player) && !IsInBankaiHos(player);
    }

    public static int GetReiryoku(Player player)
    {
        return FindIchigoRelic(player)?.StoredReiryoku ?? 0;
    }

    public static void SetReiryoku(Player player, int value)
    {
        FindIchigoRelic(player)?.SetReiryoku(value);
    }

    public static void GainReiryoku(Player player, int amount)
    {
        if (amount <= 0 || !CanGainReiryoku(player))
            return;

        SetReiryoku(player, GetReiryoku(player) + amount);
    }

    public static bool IsFull(Player player)
    {
        IchigoRelicBase? relic = FindIchigoRelic(player);

        return relic != null && relic.StoredReiryoku >= relic.MaxReiryoku;
    }

    public static void Reset(Player player)
    {
        SetReiryoku(player, 0);
    }
}
