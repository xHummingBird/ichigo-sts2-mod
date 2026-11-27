using BaseLib.Utils;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using Ichigo.IchigoCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Cards.Common;

public class VortexPull() : IchigoCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Reiryoku", 5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var enemies = CombatState.HittableEnemies.ToList();

        if (ownerCreature == null ||
            enemies.Count == 0)
        {
            return;
        }

        var firstEnemy = enemies[0];

        var dashDistance = enemies.Count > 1
            ? 150f
            : 400f;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (Owner?.Character is Character.Ichigo ichigo)
        {
            AudioHelper.PlayRandomPhrase();
            await ichigo.DashTo(
                ownerCreature,
                firstEnemy,
                distance: dashDistance,
                durationSeconds: 0.2f,
                overrideAnim: "dash");

            ichigo.PlayAnimation(
                ownerCreature,
                "arc_slash");

            SfxCmd.Play(
                "res://Ichigo/sfx/from_cloud/sword_swing_heavy.wav");

            await Task.Delay(50);

            SfxCmd.Play(
                "res://Ichigo/sfx/slash_1.wav");

            await Task.Delay(50);

            foreach (var enemy in enemies)
            {
                ichigo.PlayVfxOnTarget(
                    enemy,
                    "res://Ichigo/scenes/vfx.tscn",
                    "hit");
            }
        }

        await CommonActions.CardAttack(this, play)
            .WithHitFx(
                null,
                "res://Ichigo/sfx/from_cloud/cloud_hit.wav")
            .Execute(choiceContext);

        if (Owner?.Character is Character.Ichigo retreatingIchigo)
        {
            await retreatingIchigo.Retreat(
                ownerCreature,
                duration: 0.2f);
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        ReiryokuManager.GainReiryoku(Owner, DynamicVars["Reiryoku"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Reiryoku"].UpgradeValueBy(3m);
    }
}