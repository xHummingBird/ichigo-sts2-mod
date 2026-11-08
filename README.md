# Ichigo – Slay the Spire 2 mod

## Local setup

`Directory.Build.props` holds machine-specific paths and is git-ignored, so it is not part of the repository. After cloning (or pulling on a new machine), create it in the repository root:

```xml
<Project>
    <PropertyGroup>
        <!-- MegaDot / Godot 4.5.1 mono executable used for export-pack when publishing.
             A newer Godot version produces a .pck the game won't load. -->
        <GodotPath>/path/to/MegaDot_v4.5.1-stable_mono_linux.x86_64</GodotPath>

        <!-- Set this if the game is not found automatically. -->
        <Sts2Path>/path/to/Slay the Spire 2</Sts2Path>
        <!-- Use the data folder that matches your game build (linuxbsd or windows). -->
        <Sts2DataDir>$(Sts2Path)/data_sts2_linuxbsd_x86_64</Sts2DataDir>
    </PropertyGroup>
</Project>
```

- `GodotPath` is required only for `dotnet publish`.
- `Sts2Path` is optional if the game is found automatically (see `Sts2PathDiscovery.props`).
- Use `data_sts2_windows_x86_64` instead if the game is the Windows build.

## Build

```bash
dotnet build
```
