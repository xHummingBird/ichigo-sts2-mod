using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using HarmonyLib;
using Ichigo.IchigoCode.Cards;
using Ichigo.IchigoCode.Cards.Basic;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Powers;
using Ichigo.IchigoCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ichigo.IchigoCode.Character;

public class Ichigo : PlaceholderCharacterModel
{
    public const string CharacterId = "Ichigo";

    public static readonly Color Color = new("ffffff");

    private Vector2? _originalPosition;

    public override Color NameColor => Color;

    public override CharacterGender Gender =>
        CharacterGender.Neutral;

    public override int StartingHp => 80;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeIchigo>(),
        ModelDb.Card<StrikeIchigo>(),
        ModelDb.Card<StrikeIchigo>(),
        ModelDb.Card<StrikeIchigo>(),
        ModelDb.Card<StrikeIchigo>(),

        ModelDb.Card<DefendIchigo>(),
        ModelDb.Card<DefendIchigo>(),
        ModelDb.Card<DefendIchigo>(),
        ModelDb.Card<DefendIchigo>(),
        ModelDb.Card<DefendIchigo>(),

        ModelDb.Card<DualZangetsu>(),
        ModelDb.Card<FlashStep>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<Zangetsu>()
    ];

    public override CardPoolModel CardPool =>
        ModelDb.CardPool<IchigoCardPool>();

    public override RelicPoolModel RelicPool =>
        ModelDb.RelicPool<IchigoRelicPool>();

    public override PotionPoolModel PotionPool =>
        ModelDb.PotionPool<IchigoPotionPool>();

    // =========================================================================
    // CHARACTER UI
    // =========================================================================

    public override Control CustomIcon
    {
        get
        {
            var icon =
                NodeFactory<Control>.CreateFromResource(
                    CustomIconTexturePath);

            icon.SetAnchorsAndOffsetsPreset(
                Control.LayoutPreset.FullRect);

            return icon;
        }
    }
    
    public override CustomEnergyCounter? CustomEnergyCounter =>
        new(
            EnergyCounterPaths,
            new Color(0.2f, 0.2f, 0.2f),
            new Color(1f, 1f, 1f));

    private string EnergyCounterPaths(int i)
    {
        return i switch
        {
            1 => "charui/big_energy.png".ImagePath(),
            _ => "charui/blank.png".ImagePath()
        };
    }

    public override string CustomIconTexturePath =>
        "character_icon_ichigo.png".CharacterUiPath();

    public override string CustomCharacterSelectIconPath =>
        "char_select_ichigo.png".CharacterUiPath();

    public override string CustomCharacterSelectLockedIconPath =>
        "char_select_char_name_locked.png".CharacterUiPath();

    public override string CustomMapMarkerPath =>
        "map_marker_ichigo.png".CharacterUiPath();

    private const string CustomVisualScenePath =
        "res://Ichigo/scenes/ichigo.tscn";

    public override string CustomRestSiteAnimPath =>
        "res://Ichigo/scenes/ichigo_rest_site.tscn";

    public override string CustomCharacterSelectBg =>
        "res://Ichigo/images/charui/char_selection_bg_ichigo.tscn";

    public override string CustomMerchantAnimPath =>
        "res://Ichigo/scenes/ichigo_merchant.tscn";

    public override string CharacterSelectSfx =>
        "res://Ichigo/sounds/run_start.wav";

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        IchigoAssets.EnsurePreloaded();

        return NodeFactory<NCreatureVisuals>.CreateFromScene(
            CustomVisualScenePath);
    }

    // =========================================================================
    // FORM MANAGEMENT
    // =========================================================================

    /// <summary>
    /// Returns the suffix used by Ichigo's current form.
    ///
    /// Base:
    ///     _base
    ///
    /// True Bankai:
    ///     _bankai
    ///
    /// Horn of Salvation:
    ///     _hos
    ///
    /// Horn of Salvation is checked first in case both powers temporarily
    /// exist during a form transition.
    /// </summary>
    public string GetAnimationFormSuffix(
        Creature creature)
    {
        if (creature.HasPower<BankaiHosPower>())
            return "_hos";

        if (creature.HasPower<TrueBankaiPower>())
            return "_bankai";

        return "_base";
    }

    /// <summary>
    /// Converts a generic animation trigger into the animation belonging to
    /// Ichigo's current form.
    ///
    /// Death is shared by all forms and therefore remains "die".
    /// </summary>
    public string GetFormAnimationName(
        Creature creature,
        string trigger)
    {
        string baseAnimation =
            trigger.ToLowerInvariant() switch
            {
                "hit" => "hurt",
                "hurt" => "hurt",
                "idle" => "idle",
                "attack" => "attack",
                "block" => "block",
                "dash" => "dash",
                "retreat" => "retreat",
                "victory_before" => "victory_before",
                "victory" => "victory",
                "dead" => "die",
                "die" => "die",
                _ => trigger
            };

        if (baseAnimation == "die" ||
            baseAnimation == "victory_before" ||
            baseAnimation == "victory")
        {
            return baseAnimation;
        }

        string suffix =
            GetAnimationFormSuffix(creature);

        return $"{baseAnimation}{suffix}";
    }

    // =========================================================================
    // ANIMATION
    // =========================================================================

    public (
        float total,
        float[] impacts
    ) PlayAnimation(
        Creature creature,
        string trigger)
    {
        if (creature == null ||
            string.IsNullOrEmpty(trigger))
        {
            return (
                0f,
                Array.Empty<float>());
        }

        var node =
            NCombatRoom.Instance?
                .GetCreatureNode(creature);

        if (node?.Visuals == null)
        {
            return (
                0f,
                Array.Empty<float>());
        }

        var animPlayer =
            node.Visuals.GetNodeOrNull<AnimationPlayer>(
                "AnimationPlayer");

        if (animPlayer == null)
        {
            return (
                0f,
                Array.Empty<float>());
        }

        string animationName =
            GetFormAnimationName(
                creature,
                trigger);

        if (!animPlayer.HasAnimation(animationName))
        {
            return (
                0f,
                Array.Empty<float>());
        }

        var animation =
            animPlayer.GetAnimation(animationName);

        animPlayer.SpeedScale = 1f;
        animPlayer.Play(animationName);

        if (animationName != "die")
        {
            string idleAnimation =
                GetFormAnimationName(
                    creature,
                    "idle");

            if (animationName != idleAnimation &&
                animPlayer.HasAnimation(idleAnimation))
            {
                animPlayer.Queue(idleAnimation);
            }
        }

        return (
            (float)animation.Length,
            Array.Empty<float>());
    }

    // =========================================================================
    // DELAYS
    // =========================================================================

    public Task Delay(
        Creature creature,
        float seconds)
    {
        if (seconds <= 0f)
            return Task.CompletedTask;

        int milliseconds =
            Math.Max(
                1,
                (int)Math.Round(
                    seconds * 1000f));

        return Task.Delay(milliseconds);
    }

    public Task DelayMs(
        Creature creature,
        int milliseconds)
    {
        if (milliseconds <= 0)
            return Task.CompletedTask;

        return Task.Delay(milliseconds);
    }

    // =========================================================================
    // DASH TO
    // =========================================================================

    public async Task DashTo(
        Creature player,
        Creature target,
        float durationSeconds = 0.3f,
        float distance = 200f,
        bool dashBehind = false,
        string? overrideAnim = "dash")
    {
        var node =
            NCombatRoom.Instance?
                .GetCreatureNode(player);

        var targetNode =
            NCombatRoom.Instance?
                .GetCreatureNode(target);

        if (node == null ||
            targetNode == null)
        {
            return;
        }

        if (!_originalPosition.HasValue)
            _originalPosition = node.Position;

        if (!string.IsNullOrEmpty(overrideAnim))
        {
            PlayAnimation(
                player,
                overrideAnim);
        }

        bool playerIsLeftOfTarget =
            node.Position.X <
            targetNode.Position.X;

        Vector2 offsetDirection =
            playerIsLeftOfTarget
                ? Vector2.Left
                : Vector2.Right;

        if (dashBehind)
            offsetDirection = -offsetDirection;

        Vector2 targetPosition =
            targetNode.Position +
            offsetDirection * distance;

        if (durationSeconds <= 0f)
        {
            node.Position = targetPosition;
            return;
        }

        var tween = node.CreateTween();

        tween.TweenProperty(
                node,
                "position",
                targetPosition,
                durationSeconds)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        await node.ToSignal(
            tween,
            Tween.SignalName.Finished);
    }

    // =========================================================================
    // DASH PAST
    // =========================================================================

    public async Task DashPast(
        Creature player,
        Creature target,
        string? attackAnim = null,
        float durationSeconds = 0.3f,
        float behindDistance = 200f,
        float overshoot = 0f)
    {
        var node =
            NCombatRoom.Instance?
                .GetCreatureNode(player);

        var targetNode =
            NCombatRoom.Instance?
                .GetCreatureNode(target);

        if (node == null ||
            targetNode == null)
        {
            return;
        }

        if (!_originalPosition.HasValue)
            _originalPosition = node.Position;

        Vector2 frontDirection =
            player.Side == CombatSide.Player
                ? Vector2.Left
                : Vector2.Right;

        Vector2 behindDirection =
            -frontDirection;

        Vector2 endPosition =
            targetNode.Position +
            behindDirection *
            (behindDistance + overshoot);

        if (!string.IsNullOrEmpty(attackAnim))
        {
            PlayAnimation(
                player,
                attackAnim);
        }

        if (durationSeconds <= 0f)
        {
            node.Position = endPosition;
            return;
        }

        var tween = node.CreateTween();

        tween.TweenProperty(
                node,
                "position",
                endPosition,
                durationSeconds)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        await node.ToSignal(
            tween,
            Tween.SignalName.Finished);
    }

    // =========================================================================
    // RETREAT
    // =========================================================================

    public async Task Retreat(
        Creature player,
        string? animation = "retreat",
        bool goIdle = true,
        float duration = 0.3f)
    {
        var node =
            NCombatRoom.Instance?
                .GetCreatureNode(player);

        if (node == null ||
            !_originalPosition.HasValue)
        {
            return;
        }

        if (!string.IsNullOrEmpty(animation))
        {
            PlayAnimation(
                player,
                animation);
        }

        if (duration <= 0f)
        {
            node.Position =
                _originalPosition.Value;
        }
        else
        {
            var tween = node.CreateTween();

            tween.TweenProperty(
                    node,
                    "position",
                    _originalPosition.Value,
                    duration)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.InOut);

            await node.ToSignal(
                tween,
                Tween.SignalName.Finished);
        }

        _originalPosition = null;

        if (node.Visuals != null)
            node.Visuals.Position = Vector2.Zero;

        if (goIdle)
        {
            PlayAnimation(
                player,
                "idle");
        }
    }

    // =========================================================================
    // ORIGINAL POSITION MANAGEMENT
    // =========================================================================

    public void RememberCombatPosition(
        Creature creature)
    {
        var node =
            NCombatRoom.Instance?
                .GetCreatureNode(creature);

        if (node == null)
            return;

        if (!_originalPosition.HasValue)
            _originalPosition = node.Position;
    }

    public void ForgetCombatPosition()
    {
        _originalPosition = null;
    }

    public Vector2? GetOriginalCombatPosition()
    {
        return _originalPosition;
    }

    // =========================================================================
    // SCREEN SHAKE
    // =========================================================================

    public void DoScreenShake(
        ShakeStrength strength =
            ShakeStrength.Medium,
        ShakeDuration duration =
            ShakeDuration.Short)
    {
        NGame.Instance?.ScreenShake(
            strength,
            duration);
    }

    // =========================================================================
    // VFX
    // =========================================================================

    public Node2D? PlayVfxOnTarget(
        Creature target,
        string path,
        string animName)
    {
        var targetNode =
            NCombatRoom.Instance?
                .GetCreatureNode(target);

        if (targetNode?.Visuals == null)
            return null;

        var scene =
            GD.Load<PackedScene>(path);

        if (scene == null)
            return null;

        var vfx =
            scene.Instantiate<Node2D>();

        targetNode.Visuals.AddChild(vfx);
        vfx.Position = Vector2.Zero;

        var animPlayer =
            vfx.GetNodeOrNull<AnimationPlayer>(
                "AnimationPlayer");

        if (animPlayer != null &&
            animPlayer.HasAnimation(animName))
        {
            animPlayer.SpeedScale = 1f;
            animPlayer.Play(animName);
        }

        return vfx;
    }

    // =========================================================================
    // AUTOMATIC ANIMATION TRIGGERS
    // =========================================================================

    [HarmonyPatch(
        typeof(NCreature),
        nameof(NCreature.SetAnimationTrigger))]
    public static class NCreatureSetTriggerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(
            NCreature __instance,
            string trigger)
        {
            if (__instance.Entity?
                    .Player?
                    .Character
                is Ichigo character)
            {
                character.PlayAnimation(
                    __instance.Entity,
                    trigger);

                return false;
            }

            return true;
        }
    }

    // =========================================================================
    // DEATH ANIMATION
    // =========================================================================

    [HarmonyPatch(
        typeof(NCreature),
        nameof(NCreature.StartDeathAnim))]
    public static class IchigoStartDeathAnimPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            NCreature __instance,
            ref float __result)
        {
            if (__instance.Entity?
                    .Player?
                    .Character
                is not Ichigo character)
            {
                return;
            }
            
            AudioHelper.PlayRandomGameover();
            BattleTheme.Stop();

            var animation =
                character.PlayAnimation(
                    __instance.Entity,
                    "die");

            __result =
                animation.total > 0f
                    ? animation.total
                    : 1.5f;
        }
    }

    // =========================================================================
    // VICTORY ANIMATION
    // =========================================================================

    [HarmonyPatch(
        typeof(Hook),
        nameof(Hook.AfterCombatVictory))]
    public static class IchigoVictoryAnimationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            IRunState runState,
            CombatState? combatState)
        {
            var creatures =
                combatState?
                    .Creatures?
                    .Where(creature =>
                        creature.IsPlayer);

            if (creatures == null)
                return;

            foreach (var creature in creatures)
            {
                if (creature.Player?.Character
                    is not Ichigo character)
                {
                    continue;
                }

                var node =
                    NCombatRoom.Instance?
                        .GetCreatureNode(creature);

                var animPlayer =
                    node?
                        .Visuals?
                        .GetNodeOrNull<AnimationPlayer>(
                            "AnimationPlayer");

                if (animPlayer == null)
                    continue;

                animPlayer.SpeedScale = 1f;
                
                AudioHelper.PlayRandomVictory();

                string victoryBefore =
                    character.GetFormAnimationName(
                        creature,
                        "victory_before");

                string victory =
                    character.GetFormAnimationName(
                        creature,
                        "victory");

                if (animPlayer.HasAnimation(
                    victoryBefore))
                {
                    animPlayer.Play(
                        victoryBefore);

                    if (animPlayer.HasAnimation(
                        victory))
                    {
                        animPlayer.Queue(
                            victory);
                    }
                }
                else if (animPlayer.HasAnimation(
                    victory))
                {
                    animPlayer.Play(
                        victory);
                }
            }
        }
    }

    // =========================================================================
    // DAMAGE ANIMATION
    // =========================================================================

    [HarmonyPatch(
        typeof(Hook),
        nameof(Hook.AfterDamageReceived))]
    public static class IchigoDamageAnimationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer)
        {
            if (target.Player?.Character
                is not Ichigo character)
            {
                return;
            }

            if (dealer == null ||
                dealer.Side != CombatSide.Enemy)
            {
                return;
            }

            if (props.HasFlag(
                    ValueProp.SkipHurtAnim) ||
                props.HasFlag(
                    ValueProp.Unpowered))
            {
                return;
            }

            if (result.WasFullyBlocked &&
                result.BlockedDamage > 0)
            {
                character.PlayAnimation(
                    target,
                    "block");

                return;
            }

            if (result.UnblockedDamage <= 0 ||
                target.IsDead)
            {
                return;
            }

            character.PlayAnimation(
                target,
                "hit");
        }
    }
}