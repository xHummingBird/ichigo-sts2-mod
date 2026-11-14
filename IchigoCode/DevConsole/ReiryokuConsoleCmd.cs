using System.Linq;
using Ichigo.IchigoCode.Relics;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Ichigo.IchigoCode.DevConsole;

public class ReiryokuConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "ichigo_reiryoku";

    public override string Args => "[amount:int]";

    public override string Description => "Sets Ichigo's Reiryoku (default: full)";

    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer == null)
            return new CmdResult(success: false, "A run is currently not in progress!");

        int amount = 100;
        if (args.Length > 0 && !int.TryParse(args[0], out amount))
            return new CmdResult(success: false, "Amount must be an integer.");

        IchigoRelicBase? relic = issuingPlayer.Relics.OfType<IchigoRelicBase>().FirstOrDefault();
        if (relic == null)
            return new CmdResult(success: false, "No Ichigo relic found on this player. Is this run playing Ichigo?");

        relic.SetReiryoku(amount);
        return new CmdResult(success: true, $"Reiryoku set to {relic.StoredReiryoku}.");
    }
}
