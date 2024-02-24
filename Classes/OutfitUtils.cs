using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using TRWardrobe.Classes;

namespace TRWardrobe
{
    /// <summary>
    /// A utility class for managing outfits in the game.
    /// </summary>
    public static class OutfitUtils
    {
        // The configuration data for the outfits.
        private static MappingData _config = new();

        // The root folder for the game files.
        private static readonly string RootFolder = $"{System.Environment.CurrentDirectory}";

        // The path to the configuration JSON file.
        private static readonly string ConfigJson = Path.Combine(RootFolder, "config.json");

        // The path to the vanilla outfits.
        private static readonly string VanillaOutfitsPath = Path.Combine(RootFolder, "1\\ITEM");

        // The path to the backup of the vanilla outfits.
        private static readonly string BackupVanillaPath = Path.Combine(RootFolder, "BKP_OUTFITS");

        // The path to the modded outfits.
        private static readonly string ModdedOutfitsPath = Path.Combine(RootFolder, "MOD_OUTFITS");

        // A list of the modded outfits.
        public static List<MappingObject> ModdedOutfits => _config.ModdedOutfits;

        /// <summary>
        /// Initializes the OutfitUtils class.
        /// </summary>
        static OutfitUtils()
        {
            // Load the configuration data from the JSON file, or create a new one if it doesn't exist.
            if (File.Exists(ConfigJson))
            {
                _config = JsonSerializer.Deserialize<MappingData>(File.ReadAllText(ConfigJson));
            }
            else
            {
                _config = new MappingData();
                SaveConfig();
            }

            // Create the modded outfits folder if it doesn't exist.
            if (!Directory.Exists(ModdedOutfitsPath))
            {
                MessageBox.Show("MOD_OUTFITS folder not found. Creating MOD_OUTFITS folder.");
                Directory.CreateDirectory(ModdedOutfitsPath);
            }

            // Create the backup vanilla outfits folder if it doesn't exist.
            if (!Directory.Exists(BackupVanillaPath))
            {
                MessageBox.Show("Previous installment not found. Creating OUTFITS_PATH folder.");
                Directory.CreateDirectory(VanillaOutfitsPath);
            }
        }

        /// <summary>
        /// Initializes the outfits.
        /// </summary>
        public static void Init()
        {
            if (!IsBackup())
            {
                CreateBackup();
            }

            CheckModdedFolder();
        }

        /// <summary>
        /// Checks if a backup of the vanilla outfits exists.
        /// </summary>
        /// <returns>True if a backup exists, false otherwise.</returns>
        public static bool IsBackup() => Directory.Exists(BackupVanillaPath) &&
                                         _config.VanillaOutfits.All(o =>
                                             File.Exists(Path.Combine(BackupVanillaPath, o.Filename)));

        /// <summary>
        /// Creates a backup of the vanilla outfits.
        /// </summary>
        public static void CreateBackup()
        {
            // Create the backup folder if it doesn't exist.
            if (!Directory.Exists(BackupVanillaPath))
            {
                Directory.CreateDirectory(BackupVanillaPath);
            }

            // Copy all the vanilla outfits to the backup folder.
            foreach (var outfit in _config.VanillaOutfits)
            {
                File.Copy(Path.Combine(VanillaOutfitsPath, outfit.Filename),
                    Path.Combine(BackupVanillaPath, outfit.Filename),
                    true);
            }

            MessageBox.Show("Backup created. Total outfits: " + _config.VanillaOutfits.Count);
        }

        /// <summary>
        /// Restores the vanilla outfits from the backup.
        /// </summary>
        public static void RestoreBackup()
        {
            foreach (var outfit in _config.VanillaOutfits)
            {
                File.Copy(Path.Combine(BackupVanillaPath, outfit.Filename),
                    Path.Combine(VanillaOutfitsPath, outfit.Filename),
                    true);
            }
        }

        /// <summary>
        /// Checks the modded outfits folder and creates it if it doesn't exist.
        /// </summary>
        private static void CheckModdedFolder()
        {
            // If the modded outfits folder doesn't exist, create it.
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

            // Update the modded outfits in the configuration data.
            foreach (var file in _config.ModdedOutfits)
            {
                if (!file.Key.Contains("_MODDED"))
                {
                    file.Key += "_MODDED";
                }

                Console.WriteLine(file.Filename);
            }

            // Save the updated configuration data.
            SaveConfig();
        }

        /// <summary>
        /// Saves the configuration data to the JSON file.
        /// </summary>
        private static void SaveConfig()
        {
            File.WriteAllText(ConfigJson, JsonSerializer.Serialize(_config, new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = true
            }));
        }

        /// <summary>
        /// Unsets an outfit.
        /// </summary>
        /// <param name="key">The key of the outfit to unset.</param>
        /// <param name="isModded">Whether the outfit is modded.</param>
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

        /// <summary>
        /// Sets an outfit.
        /// </summary>
        /// <param name="key">The key of the outfit to set.</param>
        /// <param name="isModded">Whether the outfit is modded.</param>
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

        /// <summary>
        /// Applies a texture to an outfit.
        /// </summary>
        /// <param name="texture">The texture to apply.</param>
        /// <param name="isModded">Whether the outfit is modded.</param>
        /// <param name="key">The key of the outfit.</param>
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

                File.Copy($"{ModdedOutfitsPath}\\{validatedKey}\\{i}\\TEX\\{texture}",
                    $"{RootFolder}\\{i}\\TEX\\{texture}",
                    true);
                Console.WriteLine(
                    $"Copying {ModdedOutfitsPath}\\{validatedKey}\\{i}\\TEX\\{texture} to {RootFolder}\\{i}\\TEX\\{texture}");
            }
        }

        /// <summary>
        /// Creates a backup of a texture.
        /// </summary>
        /// <param name="texture">The texture to backup.</param>
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

        /// <summary>
        /// Restores a texture from a backup.
        /// </summary>
        /// <param name="texture">The texture to restore.</param>
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

        /// <summary>
        /// Resets all textures to their original state.
        /// </summary>
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
}