using HarmonyLib;
using Ichigo.IchigoCode.Cards.Ancient;
using Ichigo.IchigoCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Ichigo.IchigoCode.Extensions;

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
internal static class IchigoTouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is Zangetsu)
        {
            __result = ModelDb.Relic<TensaZangetsu>().ToMutable();
        }
    }
}

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
internal static class IchigoArchaicToothTranscendencePatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<Cards.Basic.DualZangetsu>().Id] = ModelDb.Card<TrueZangetsu>();
    }
}

[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
public static class IchigoDustyTomeSetupPatch
{
    [HarmonyPostfix]
    public static void Postfix(DustyTome __instance, Player player)
    {
        if (player.Character is not Character.Ichigo)
            return;

        __instance.AncientCard = ModelDb.Card<BankaiTensaZangetsu>().Id;
    }
}


[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
public static class IchigoDustyTomePatch
{
    [HarmonyPrefix]
    public static void Prefix(DustyTome __instance)
    {
        if (__instance.Owner?.Character is not Character.Ichigo)
            return;
        
        __instance.AncientCard = ModelDb.Card<BankaiTensaZangetsu>().Id;
    }
}