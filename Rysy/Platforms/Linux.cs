using Rysy.Mods;

namespace Rysy.Platforms;

public class Linux : RysyPlatform {
    private static string SaveLocation = UncachedGetSaveLocation();

    public override string GetSaveLocation() => RysyState.CmdArguments.Portable ? "portableData" : SaveLocation;

    private static string UncachedGetSaveLocation() {
        // from FNA wiki
        string osConfigDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME")!;
        if (string.IsNullOrEmpty(osConfigDir)) {
            osConfigDir = Environment.GetEnvironmentVariable("HOME")!;
            if (string.IsNullOrEmpty(osConfigDir)) {
                return "."; // Oh well.
            }
            osConfigDir += "/.local/share";
        }
        return Path.Combine(osConfigDir, "Rysy");
    }

    public override void Init() {
        base.Init();

        Logger.UseColorsInConsole = true;
    }

    private LayeredFilesystem? _fontFilesystem;
    
    public override IModFilesystem? GetSystemFontsFilesystem() {
        if (_fontFilesystem is { })
            return _fontFilesystem;

        _fontFilesystem = new LayeredFilesystem();

        void AddIfExists(string path) {
            if (Path.Exists(path))
                _fontFilesystem!.AddFilesystem(new FolderModFilesystem(path), path);
        }

        AddIfExists("/usr/share/fonts");
        AddIfExists("/usr/local/share/fonts");

        // NixOS does not have /usr/share or /usr/local; but by enabling fonts.fontDir.enable this directory can be used:
        AddIfExists("/run/current-system/sw/share/X11/fonts");

        return _fontFilesystem;
    }
}
