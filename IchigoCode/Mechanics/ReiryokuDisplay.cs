using System.Linq;
using Godot;
using HarmonyLib;
using Ichigo.IchigoCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Ichigo.IchigoCode.Mechanics;

public partial class ReiryokuDisplay : Control
{
    public static ReiryokuDisplay? Instance
    {
        get;
        private set;
    }

    private const string ScenePath =
        "res://Ichigo/scenes/reiryoku_display.tscn";

    /*
     * Position of the entire Reiryoku display
     * relative to Ichigo's energy counter.
     */
    private static readonly Vector2 DisplayPosition =
        new(-75f, -75f);

    /*
     * Additional position offset for only the number.
     *
     * Negative X = left
     * Positive X = right
     * Negative Y = up
     * Positive Y = down
     */
    private static readonly Vector2 LabelPositionOffset =
        new(0f, 14f);

    /*
     * Ichigo has only one display state.
     * This texture remains visible regardless of the
     * current amount of Reiryoku.
     */
    private static readonly Texture2D ReiryokuTexture =
        GD.Load<Texture2D>(
            "res://Ichigo/images/charui/reiryoku.png");

    /*
     * Hover-tip position relative to the display.
     */
    private static readonly Vector2 HoverTipOffset =
        new(-60f, -450f);

    private Control? _display;
    private TextureRect? _overlay;
    private RichTextLabel? _reiryokuLabel;

    private Player? _player;

    private IHoverTip? _reiryokuHoverTip;

    private int _lastReiryoku =
        int.MinValue;

    public override void _Ready()
    {
        Instance = this;

        Name =
            "IchigoReiryokuDisplay";

        MouseFilter =
            MouseFilterEnum.Pass;

        CallDeferred(
            nameof(Setup));
    }

    private async void Setup()
    {
        if (!IsInsideTree())
            return;

        /*
         * Wait for the local combat player.
         */
        for (int i = 0; i < 60; i++)
        {
            var state =
                CombatManager.Instance?
                    .DebugOnlyGetState();

            Player? player =
                state?.Players.FirstOrDefault(
                    currentPlayer =>
                        LocalContext.IsMe(currentPlayer));

            if (player != null)
            {
                if (player.Character
                    is not Character.Ichigo)
                {
                    QueueFree();
                    return;
                }

                _player =
                    player;

                break;
            }

            SceneTree? tree =
                GetTree();

            if (tree == null)
                return;

            await ToSignal(
                tree,
                SceneTree.SignalName.ProcessFrame);
        }

        if (_player == null)
        {
            QueueFree();
            return;
        }

        PackedScene? scene =
            GD.Load<PackedScene>(
                ScenePath);

        if (scene == null)
        {
            GD.PushError(
                $"[Ichigo Reiryoku] Failed to load {ScenePath}");

            QueueFree();
            return;
        }

        _display =
            scene.Instantiate<Control>();

        AddChild(
            _display);

        _display.SetAnchorsPreset(
            LayoutPreset.BottomLeft);

        _display.Position =
            DisplayPosition;

        GetSceneNodes();

        if (!ValidateRequiredNodes())
        {
            QueueFree();
            return;
        }

        ConfigureDisplay();
        ConfigureLabel();
        ConfigureHoverTip();

        ResetDisplayState();
        UpdateDisplay();
    }

    private void GetSceneNodes()
    {
        if (_display == null)
            return;

        _overlay =
            _display.GetNodeOrNull<TextureRect>(
                "Overlay");

        _reiryokuLabel =
            _display.GetNodeOrNull<RichTextLabel>(
                "RichTextLabel");
    }

    private bool ValidateRequiredNodes()
    {
        bool valid =
            _display != null &&
            _overlay != null &&
            _reiryokuLabel != null;

        if (!valid)
        {
            GD.PushError(
                "[Ichigo Reiryoku] reiryoku_display.tscn must " +
                "contain Overlay and RichTextLabel.");
        }

        return valid;
    }

    private void ConfigureDisplay()
    {
        if (_display == null ||
            _overlay == null)
        {
            return;
        }

        _display.MouseFilter =
            MouseFilterEnum.Stop;

        /*
         * Ichigo has only one visual state.
         * The same texture is used at every Reiryoku value.
         */
        _overlay.Texture =
            ReiryokuTexture;

        _overlay.Visible =
            true;

        /*
         * Prevent the image from blocking mouse input
         * to the root Control.
         */
        _overlay.MouseFilter =
            MouseFilterEnum.Ignore;

        _display.MouseEntered +=
            OnReiryokuHovered;

        _display.MouseExited +=
            OnReiryokuUnhovered;
    }

    private void PlayGainAnimation()
    {
        if (_reiryokuLabel == null)
            return;

        /*
         * Ensure the number grows from its center instead
         * of expanding toward the bottom-right.
         */
        _reiryokuLabel.PivotOffset =
            _reiryokuLabel.Size / 2f;

        _reiryokuLabel.Scale =
            Vector2.One;

        Tween tween =
            CreateTween();

        tween.SetParallel();

        tween.TweenProperty(
            _reiryokuLabel,
            "scale",
            new Vector2(1.35f, 1.35f),
            0.08f);

        tween.TweenProperty(
                _reiryokuLabel,
                "scale",
                Vector2.One,
                0.12f)
            .SetDelay(0.08f);
    }

    private void ConfigureLabel()
    {
        if (_reiryokuLabel == null)
            return;

        _reiryokuLabel.Visible =
            true;

        _reiryokuLabel.MouseFilter =
            MouseFilterEnum.Ignore;

        Font? font =
            GD.Load<Font>(
                "res://themes/kreon_bold_shared.tres");

        if (font != null)
        {
            _reiryokuLabel.AddThemeFontOverride(
                "font",
                font);

            _reiryokuLabel.AddThemeFontOverride(
                "normal_font",
                font);
        }

        _reiryokuLabel.AddThemeFontSizeOverride(
            "normal_font_size",
            24);

        _reiryokuLabel.AddThemeColorOverride(
            "default_color",
            Colors.White);

        _reiryokuLabel.AddThemeColorOverride(
            "font_outline_color",
            new Color(
                0.03f,
                0.03f,
                0.03f));

        _reiryokuLabel.AddThemeConstantOverride(
            "outline_size",
            7);

        _reiryokuLabel.BbcodeEnabled =
            true;

        _reiryokuLabel.FitContent =
            true;

        _reiryokuLabel.Position +=
            LabelPositionOffset;

        /*
         * The final size is normally established after
         * the scene has entered the tree. Update the pivot
         * again before each gain animation.
         */
        _reiryokuLabel.PivotOffset =
            _reiryokuLabel.Size / 2f;
    }

    private void ConfigureHoverTip()
    {
        _reiryokuHoverTip =
            IchigoStaticHoverTips.Reiryoku;
    }

    public override void _Process(
        double delta)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_player == null ||
            _reiryokuLabel == null)
        {
            return;
        }

        int reiryoku =
            ReiryokuManager.GetReiryoku(
                _player);

        if (reiryoku == _lastReiryoku)
            return;

        SetReiryokuDisplay(
            reiryoku);
    }

    private void SetReiryokuDisplay(
        int reiryoku)
    {
        int previous =
            _lastReiryoku;

        _lastReiryoku =
            Mathf.Max(
                reiryoku,
                0);

        /*
         * Do not play the gain animation during initial setup.
         * Only animate an actual increase after the first value
         * has been recorded.
         */
        if (previous != int.MinValue &&
            _lastReiryoku > previous)
        {
            PlayGainAnimation();
        }

        if (_overlay == null ||
            _reiryokuLabel == null)
        {
            return;
        }

        /*
         * There is no alternate full state.
         * The same texture and number are always displayed.
         */
        _overlay.Texture =
            ReiryokuTexture;

        _overlay.Visible =
            true;

        _reiryokuLabel.Visible =
            true;

        _reiryokuLabel.Text =
            $"[center]{_lastReiryoku}[/center]";
    }

    private void OnReiryokuHovered()
    {
        if (_display == null ||
            _reiryokuHoverTip == null)
        {
            return;
        }

        NHoverTipSet.Clear();

        Control? tip =
            NHoverTipSet.CreateAndShow(
                _display,
                _reiryokuHoverTip);

        if (tip == null)
            return;

        tip.GlobalPosition =
            _display.GlobalPosition +
            HoverTipOffset;

        tip.MouseFilter =
            MouseFilterEnum.Ignore;
    }

    private void OnReiryokuUnhovered()
    {
        if (_display == null)
            return;

        NHoverTipSet.Remove(
            _display);
    }

    private void ResetDisplayState()
    {
        _lastReiryoku =
            int.MinValue;

        if (_overlay != null)
        {
            _overlay.Texture =
                ReiryokuTexture;

            _overlay.Visible =
                true;
        }

        if (_reiryokuLabel != null)
        {
            _reiryokuLabel.Visible =
                true;

            _reiryokuLabel.Scale =
                Vector2.One;

            _reiryokuLabel.Text =
                "[center]0[/center]";
        }
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance =
                null;
        }

        if (_display != null)
        {
            _display.MouseEntered -=
                OnReiryokuHovered;

            _display.MouseExited -=
                OnReiryokuUnhovered;

            NHoverTipSet.Remove(
                _display);
        }

        _reiryokuHoverTip =
            null;

        _player =
            null;

        _overlay =
            null;

        _reiryokuLabel =
            null;

        _display =
            null;
    }
}

[HarmonyPatch(
    typeof(NEnergyCounter),
    nameof(NEnergyCounter._Ready))]
public static class IchigoReiryokuDisplayOverlayPatch
{
    public static void Postfix(
        NEnergyCounter __instance)
    {
        if (__instance == null)
            return;

        if (!GodotObject.IsInstanceValid(
                __instance))
        {
            return;
        }

        var state =
            CombatManager.Instance?
                .DebugOnlyGetState();

        Player? player =
            state?.Players.FirstOrDefault(
                currentPlayer =>
                    LocalContext.IsMe(currentPlayer));

        if (player?.Character
            is not Character.Ichigo)
        {
            return;
        }

        if (__instance
                .GetNodeOrNull<ReiryokuDisplay>(
                    "ReiryokuDisplay") != null)
        {
            return;
        }

        __instance.AddChild(
            new ReiryokuDisplay
            {
                Name =
                    "ReiryokuDisplay"
            });
    }
}