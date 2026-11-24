using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Relics;

namespace Ichigo.IchigoCode.Extensions;

public class IchigoExtensions
{
    public static class CombatHelpers
    {
        private const string DefaultHitVfxScene =
            "res://Ichigo/scenes/vfx.tscn";

        private const string DefaultHitVfx = "hit";

        public const string NoHitVfx = "__NONE__";

        public static async Task IchigoFakeHit(
            Creature attacker,
            Creature target,
            string? attackSfx = null,
            string? hitSfx = null,
            string? hitVfxScene = DefaultHitVfxScene,
            string? hitVfx = null,
            bool playHitVfx = true)
        {
            if (target == null)
                return;

            if (attackSfx != null)
                SfxCmd.Play(attackSfx);

            if (hitSfx != null)
                SfxCmd.Play(hitSfx);

            var ichigo = attacker.Player.Character as Character.Ichigo;

            if (playHitVfx && ichigo != null)
            {
                ichigo.PlayVfxOnTarget(
                    target,
                    hitVfxScene ?? DefaultHitVfxScene,
                    hitVfx ?? DefaultHitVfx);
            }

            await CreatureCmd.TriggerAnim(
                target,
                "Hit",
                0f);

            if (target.Monster?.HasHurtSfx == true)
                SfxCmd.Play(target.Monster.HurtSfx);
        }
    }
}