using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Relics;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Runs;

namespace Ichigo.IchigoCode.Cards.Ancient;

public class TrueBankai() : IchigoCard(
    0,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Turns", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TrueBankaiPower>()
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (LocalContext.IsMe(Owner.Creature))
            BattleTheme.PlayBankaiMusic(RunManager.Instance.NetService.NetId);
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        var ichigo = Owner.Character as Character.Ichigo;
        AudioHelper.PlayRandomBankaiBefore();
        ichigo.PlayCutIn("jujisho_bankai");
        SfxCmd.Play("res://Ichigo/sfx/cutin_sfx.wav");
        await Task.Delay(1000);
        ichigo.PlayAnimation(Owner.Creature,"bankai_true");
        AudioHelper.PlayRandomBankai();
        SfxCmd.Play("res://Ichigo/sfx/from_cloud/limit_break_thunder.wav");
        await Task.Delay(600);
        SfxCmd.Play("res://Ichigo/sfx/getsuga_5.wav", 0.7f);
        await PowerCmd.Apply<TrueBankaiPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Turns"].IntValue,
            Owner.Creature,
            this);
        await Task.Delay(900);
        ichigo.PlayAnimation(Owner.Creature, "idle");
        if (IchigoConfig.EnableVoice)
            SfxCmd.Play("res://Ichigo/sounds/tensa_zangetsu.mp3", 0.7f);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Turns"].UpgradeValueBy(1);
    }
}
