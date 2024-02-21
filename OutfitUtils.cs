using System.IO;
using System.Windows;

namespace TRWardrobe;

public static class OutfitUtils
{
    public const string OUTFITS_PATH = ".\\1\\ITEM";

    public const string BKP_FOLDER = ".\\BKP_OUTFITS";

    public static Dictionary<string, string> OUTFITS => new()
    {
        { "classic", "OUTFIT_TR1_CLASSIC.TRM" },
        { "tr1training", "OUTFIT_TR1_GYM.TRM" },
        { "tr2training", "OUTFIT_TR2_GYM.TRM" },
        { "tr3training", "OUTFIT_TR3_GYM.TRM" },
        { "tr2classic", "OUTFIT_TR2_CLASSIC.TRM" },
        { "tr2home", "OUTFIT_TR2_HOME.TRM" },
        { "swim", "OUTFIT_TR2_SWIM.TRM" },
        { "winter", "OUTFIT_TR2_TIBET.TRM" },
        { "antar", "OUTFIT_TR3_ANTARC.TRM" },
        { "cat", "OUTFIT_TR3_CATSUIT.TRM" },
        { "pacific", "OUTFIT_TR3_COAST.TRM" },
        { "nevada", "OUTFIT_TR3_NEVADA.TRM" }
    };

    public static bool IsBackup()
    {
        return Directory.Exists(BKP_FOLDER) && OUTFITS.All(o => File.Exists(Path.Combine(BKP_FOLDER, o.Value)));
    }

    public static void CreateBackup()
    {
        // Create BKP_FOLDER if it doesn't exist
        if (!Directory.Exists(BKP_FOLDER))
        {
            Directory.CreateDirectory(BKP_FOLDER);
        }

        // Copy all TRM files to BKP_FOLDER from OUTFITS_PATH
        foreach (var outfit in OUTFITS)
        {
            File.Copy(Path.Combine(OUTFITS_PATH, outfit.Value), Path.Combine(BKP_FOLDER, outfit.Value), true);
        }

        MessageBox.Show("Backup created. Total outfits: " + OUTFITS.Count);
    }

    public static void RestoreBackup()
    {
        foreach (var outfit in OUTFITS)
        {
            File.Copy(Path.Combine(BKP_FOLDER, outfit.Value), Path.Combine(OUTFITS_PATH, outfit.Value), true);
        }
    }

    public static void SetOutfit(string key)
    {
        string outfit = OUTFITS[key];

        // Remove all the outfit TRM files from the OUTFITS_PATH
        foreach (var orgOutfit in OUTFITS.Values)
        {
            if (!File.Exists(Path.Combine(OUTFITS_PATH, orgOutfit)))
            {
                continue;
            }

            File.Delete(Path.Combine(OUTFITS_PATH, orgOutfit));
        }

        // Copy the outfit TRM file from BKP_FOLDER to OUTFITS_PATH
        foreach (var orgOutfit in OUTFITS.Values)
        {
            File.Copy(Path.Combine(BKP_FOLDER, outfit), Path.Combine(OUTFITS_PATH, orgOutfit), true);
        }

        MessageBox.Show("Outfit set to " + key);
    }
}