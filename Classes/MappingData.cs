namespace TRWardrobe.Classes;

public class MappingData
{
    public string CurrentOutfit { get; set; } = "";

    public List<MappingObject> VanillaOutfits { get; set; } = new()
    {
        new MappingObject("tr1classic", "OUTFIT_TR1_CLASSIC.TRM", "[TR1] Classic"),
        new MappingObject("tr1training", "OUTFIT_TR1_GYM.TRM", "[TR1] Training"),
        new MappingObject("tr2training", "OUTFIT_TR2_GYM.TRM", "[TR2] Training"),
        new MappingObject("tr3training", "OUTFIT_TR3_GYM.TRM", "[TR2] Classic"),
        new MappingObject("tr2classic", "OUTFIT_TR2_CLASSIC.TRM", "[TR2] Winter Jacket"),
        new MappingObject("tr2home", "OUTFIT_TR2_HOME.TRM", "[TR2] Swim Suit"),
        new MappingObject("swim", "OUTFIT_TR2_SWIM.TRM", "[TR2] Home"),
        new MappingObject("winter", "OUTFIT_TR2_TIBET.TRM", "[TR3] Training"),
        new MappingObject("antar", "OUTFIT_TR3_ANTARC.TRM", "[TR3] Antarctica"),
        new MappingObject("cat", "OUTFIT_TR3_CATSUIT.TRM", "[TR3] Cat Suit"),
        new MappingObject("pacific", "OUTFIT_TR3_COAST.TRM", "[TR3] Pacific"),
        new MappingObject("nevada", "OUTFIT_TR3_NEVADA.TRM", "[TR3] Nevada"),
    };

    public List<MappingObject> ModdedOutfits { get; set; } = new();
}

public class MappingObject
{
    public string Key { get; set; } = "";
    public string Filename { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool IsOnlyTexture { get; set; } = false;
    public List<string>? Textures { get; set; } = null;

    public MappingObject(string key, string filename, string displayName)
    {
        Key = key;
        Filename = filename;
        DisplayName = displayName;
    }
}