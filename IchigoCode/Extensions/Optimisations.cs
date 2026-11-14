using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Ichigo.IchigoCode.Extensions;

public static class IchigoAssets
{
    private static PackedScene? _ichigoScene;
    private static PackedScene? _vfxScene;

    private const string IchigoScenePath = "res://Ichigo/scenes/ichigo.tscn";
    private const string VfxPath = "res://Ichigo/scenes/vfx.tscn";
    
    
    public static bool IsIchigoInRun(IRunState runState)
    {
        return runState?.Players?.Any(
            p => p?.Character is Ichigo.IchigoCode.Character.Ichigo
        ) ?? false;
    }

    public static PackedScene? IchigoScene
    {
        get
        {
            _ichigoScene = LoadOrReload(_ichigoScene, IchigoScenePath, "Ichigo scene");
            return _ichigoScene;
        }
    }

    public static PackedScene? VfxScene
    {
        get
        {
            _vfxScene = LoadOrReload(_vfxScene, VfxPath, "VFX");
            return _vfxScene;
        }
    }
    
    private static PackedScene? LoadOrReload(PackedScene? cachedScene, string path, string label)
    {
        if (cachedScene != null && GodotObject.IsInstanceValid(cachedScene))
            return cachedScene;

        GD.Print($"IchigoAssets: Loading {label} from {path}");

        var scene = GD.Load<PackedScene>(path);

        if (scene == null)
        {
            GD.PrintErr($"IchigoAssets: FAILED to load {label}: {path}");
            return null;
        }

        GD.Print($"IchigoAssets: Loaded {label}");
        return scene;
    }

    public static void EnsurePreloaded()
    {
        _ = IchigoScene;
        _ = VfxScene;

        GD.Print("IchigoAssets: EnsurePreloaded finished");
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterActEntered))]
public static class IchigoAfterActEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState)
    {
        // var player = runState?.Players?.FirstOrDefault();
        //
        // if (player?.Character is not Character.Ichigo)
        //     return;
        if (!IchigoAssets.IsIchigoInRun(runState))
            return;
        
        GD.Print("AfterActEntered: Ichigo detected → preloading");

        IchigoAssets.EnsurePreloaded();
    }
}


[HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
public static class IchigoAfterRoomEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState, AbstractRoom room)
    {
        // var player = runState?.Players?.FirstOrDefault();
        //
        // if (player?.Character is not Character.Ichigo)
        //     return;
        if (!IchigoAssets.IsIchigoInRun(runState))
            return;
        
        GD.Print($"AfterRoomEntered: Ichigo detected → preloading. Room = {room.GetType().Name}");

        IchigoAssets.EnsurePreloaded();
    }
}
