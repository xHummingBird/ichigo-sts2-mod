using MegaCrit.Sts2.Core.Entities.Players;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Relics;

namespace Ichigo.IchigoCode.Mechanics;

public static class ReiryokuManager
{
    private static SummonRelicBase? FindSummonRelic(Player player)
    {
        return player.Relics
            .OfType<SummonRelicBase>()
            .FirstOrDefault();
    }

    public static bool IsInBankai(Player player)
    {
        return player.Creature.HasPower<BankaiPower>();
    }

    public static bool CanGainReiryoku(Player player)
    {
        return !IsInBankai(player);
    }

    public static int GetReiryoku(Player player)
    {
        return FindSummonRelic(player)?.StoredReiryoku ?? 0;
    }

    public static void SetReiryoku(Player player, int value)
    {
        FindSummonRelic(player)?.SetReiryoku(value);
    }

    public static void GainReiryoku(Player player, int amount)
    {
        if (amount <= 0 || !CanGainReiryoku(player))
            return;

        SetReiryoku(player, GetReiryoku(player) + amount);
    }

    public static bool IsFull(Player player)
    {
        SummonRelicBase? relic = FindSummonRelic(player);

        return relic != null && relic.StoredReiryoku >= relic.MaxReiryoku;
    }

    public static void Reset(Player player)
    {
        SetReiryoku(player, 0);
    }
}
