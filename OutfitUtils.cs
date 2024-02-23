using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using TRWardrobe.Classes;

namespace TRWardrobe;

public static class OutfitUtils
{
    private static MappingData _config = new();

    // LIVE
    private static readonly string RootFolder = $"{System.Environment.CurrentDirectory}";

    // DEV
    // private static readonly string RootFolder = $@"h:\SteamLibrary\steamapps\common\Tomb Raider I-III Remastered";


    private static readonly string ConfigJson = Path.Combine(RootFolder, "config.json");
    private static readonly string VanillaOutfitsPath = Path.Combine(RootFolder, "1\\ITEM");
    private static readonly string BackupVanillaPath = Path.Combine(RootFolder, "BKP_OUTFITS");
    private static readonly string ModdedOutfitsPath = Path.Combine(RootFolder, "MOD_OUTFITS");

    public static List<MappingObject> ModdedOutfits => _config.ModdedOutfits;

    static OutfitUtils()
    {
        if (File.Exists(ConfigJson))
        {
            _config = JsonSerializer.Deserialize<MappingData>(File.ReadAllText(ConfigJson));
        }
        else
        {
            _config = new MappingData();
            // INDENT and do not serialize nulls
            SaveConfig();
        }

        if (!Directory.Exists(ModdedOutfitsPath))
        {
            MessageBox.Show("MOD_OUTFITS folder not found. Creating MOD_OUTFITS folder.");
            Directory.CreateDirectory(ModdedOutfitsPath);
        }

        if (!Directory.Exists(BackupVanillaPath))
        {
            MessageBox.Show("Previous installment not found. Creating OUTFITS_PATH folder.");
            Directory.CreateDirectory(VanillaOutfitsPath);
        }
    }

    public static void Init()
    {
        if (!IsBackup())
        {
            CreateBackup();
        }

        CheckModdedFolder();
    }

    public static bool IsBackup() => Directory.Exists(BackupVanillaPath) &&
                                     _config.VanillaOutfits.All(o =>
                                         File.Exists(Path.Combine(BackupVanillaPath, o.Filename)));

    public static void CreateBackup()
    {
        // Create BKP_FOLDER if it doesn't exist
        if (!Directory.Exists(BackupVanillaPath))
        {
            Directory.CreateDirectory(BackupVanillaPath);
        }

        // Copy all TRM files to BKP_FOLDER from OUTFITS_PATH
        foreach (var outfit in _config.VanillaOutfits)
        {
            File.Copy(Path.Combine(VanillaOutfitsPath, outfit.Filename),
                Path.Combine(BackupVanillaPath, outfit.Filename),
                true);
        }

        MessageBox.Show("Backup created. Total outfits: " + _config.VanillaOutfits.Count);
    }

    public static void RestoreBackup()
    {
        foreach (var outfit in _config.VanillaOutfits)
        {
            File.Copy(Path.Combine(BackupVanillaPath, outfit.Filename),
                Path.Combine(VanillaOutfitsPath, outfit.Filename),
                true);
        }
    }

    // MODDED OUTFITS


    private static void CheckModdedFolder()
    {
        if (!Directory.Exists(ModdedOutfitsPath))
        {
            Directory.CreateDirectory(ModdedOutfitsPath);
            MessageBox.Show(
                "MOD_OUTFITS folder created successfully. You can now add your modded outfits to this folder. Currently there are no modded outfits.");
        }
        else
        {
            MessageBox.Show(
                $"MOD_OUTFITS folder already exists and contains {Directory.GetFiles(ModdedOutfitsPath, "*.TRM").Length} modded outfits.");
        }

        foreach (var file in _config.ModdedOutfits)
        {
            if (!file.Key.Contains("_MODDED"))
            {
                file.Key += "_MODDED";
            }

            Console.WriteLine(file.Filename);
        }

        // Save the _config
        SaveConfig();
    }

    private static void SaveConfig()
    {
        File.WriteAllText(ConfigJson, JsonSerializer.Serialize(_config, new JsonSerializerOptions
        {
            WriteIndented = true,
            IgnoreNullValues = true
        }));
    }


    public static void UnsetOutfit(string key, bool isModded = false)
    {
        // Reset textures if necessary 
        var outfitMapping = isModded ? _config.ModdedOutfits : _config.VanillaOutfits;
        var mappingObject = outfitMapping.FirstOrDefault(o => o.Key == key);
        if (mappingObject == null)
        {
            return;
        }

        if (mappingObject.Textures == null)
        {
            return;
        }

        foreach (var texture in mappingObject.Textures)
        {
            RestoreTextures(texture);
        }
    }

    public static void SetOutfit(string key, bool isModded = false)
    {
        var outfitMapping = isModded ? _config.ModdedOutfits : _config.VanillaOutfits;

        MappingObject? mappingObject = outfitMapping.FirstOrDefault(o => o.Key == key);

        if (mappingObject == null)
        {
            MessageBox.Show($"Outfit {key} not found.");
            return;
        }

        if (_config.CurrentOutfit == mappingObject.Key)
        {
            MessageBox.Show($"Outfit {mappingObject.DisplayName} already set.");
            return;
        }

        // unset the current outfit and set the default textures from that
        if (!string.IsNullOrEmpty(_config.CurrentOutfit))
        {
            UnsetOutfit(_config.CurrentOutfit);
        }

        _config.CurrentOutfit = mappingObject.Key;

        var source = Path.Combine((isModded) ? ModdedOutfitsPath : BackupVanillaPath, mappingObject.Filename);

        if (!mappingObject.IsOnlyTexture)
        {
            foreach (var originalOutfit in _config.VanillaOutfits)
            {
                File.Copy(source, Path.Combine(VanillaOutfitsPath, originalOutfit.Filename), true);
                Console.WriteLine($"Copying {source} to {VanillaOutfitsPath}\\{originalOutfit.Filename}");
            }

            if (mappingObject.Textures == null)
            {
                MessageBox.Show($"Outfit set to {mappingObject.DisplayName} | No textures replaced.");
                return;
            }
        }

        if (mappingObject.Textures == null)
        {
            return;
        }

        var textures = mappingObject.Textures;
        // Create backup for each texture 
        foreach (var texture in textures)
        {
            BackupTexture(texture);
            ApplyTexture(texture, isModded, mappingObject.Key);
        }
    }

    private static void ApplyTexture(string texture, bool isModded = false, string key = "")
    {
        if (!isModded)
        {
            RestoreTextures(texture);
            return;
        }   

        var validatedKey = key.Replace("_MODDED", "");

        for (int i = 1; i < 4; i++)
        {
            if (!File.Exists($"{ModdedOutfitsPath}\\{validatedKey}\\{i}\\TEX\\{texture}"))
            {
                continue;
            }

            File.Copy($"{ModdedOutfitsPath}\\{validatedKey}\\{i}\\TEX\\{texture}", $"{RootFolder}\\{i}\\TEX\\{texture}",
                true);
            Console.WriteLine(
                $"Copying {ModdedOutfitsPath}\\{validatedKey}\\{i}\\TEX\\{texture} to {RootFolder}\\{i}\\TEX\\{texture}");
        }
    }

    private static void BackupTexture(string texture)
    {
        // Check 1/TEX, 2/TEX, 3/TEX
        for (int i = 1; i < 4; i++)
        {
            if (!File.Exists($"{RootFolder}\\{i}\\TEX\\{texture}"))
            {
                continue;
            }

            if (File.Exists($"{RootFolder}\\{i}\\TEX\\{texture}.bak"))
            {
                Console.WriteLine($"Backup already exists for {i}\\TEX\\{texture}");
                continue;
            }

            File.Copy($"{RootFolder}\\{i}\\TEX\\{texture}", $"{RootFolder}\\{i}\\TEX\\{texture}.bak", true);
            Console.WriteLine($"Creating backup {i}\\TEX\\{texture} to {i}\\TEX\\{texture}.bak");
        }
    }

    private static void RestoreTextures(string texture)
    {
        for (int i = 1; i < 4; i++)
        {
            if (!File.Exists($"{RootFolder}\\{i}\\TEX\\{texture}.bak"))
            {
                continue;
            }

            File.Copy($"{RootFolder}\\{i}\\TEX\\{texture}.bak", $"{RootFolder}\\{i}\\TEX\\{texture}", true);

            Console.WriteLine($"Copying {i}\\TEX\\{texture}.bak to {i}\\TEX\\{texture}");
        }
    }

    public static void ResetTextures()
    {
        // Reset all textures
        // Copy all .bak files to DDS
        StringBuilder sb = new();
        for (int i = 1; i < 4; i++)
        {
            foreach (var file in Directory.GetFiles($"{RootFolder}\\{i}\\TEX", "*.bak"))
            {
                File.Copy(file, file.Replace(".bak", ""), true);
                sb.AppendLine("Restoring " + file.Replace(".bak", ""));
            }
        }

        MessageBox.Show(sb.ToString());
    }

   
}