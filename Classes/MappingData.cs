namespace TRWardrobe.Classes
{
    /// <summary>
    /// Represents the mapping data for outfits.
    /// </summary>
    public class MappingData
    {
        /// <summary>
        /// Gets or sets the current outfit.
        /// </summary>
        public string CurrentOutfit { get; set; } = "";

        /// <summary>
        /// Gets or sets the list of vanilla outfits.
        /// </summary>
        public List<MappingObject> VanillaOutfits { get; set; } = new()
        {
            // Initialize the list with some default outfits.
        };

        /// <summary>
        /// Gets or sets the list of modded outfits.
        /// </summary>
        public List<MappingObject> ModdedOutfits { get; set; } = new();
    }

    /// <summary>
    /// Represents an outfit mapping object.
    /// </summary>
    public class MappingObject
    {
        /// <summary>
        /// Gets or sets the key of the outfit.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// Gets or sets the filename of the outfit.
        /// </summary>
        public string Filename { get; set; } = "";

        /// <summary>
        /// Gets or sets the display name of the outfit.
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether the outfit is only a texture.
        /// </summary>
        public bool IsOnlyTexture { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of textures for the outfit.
        /// </summary>
        public List<string>? Textures { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingObject"/> class.
        /// </summary>
        /// <param name="key">The key of the outfit.</param>
        /// <param name="filename">The filename of the outfit.</param>
        /// <param name="displayName">The display name of the outfit.</param>
        public MappingObject(string key, string filename, string displayName)
        {
            Key = key;
            Filename = filename;
            DisplayName = displayName;
        }
    }
}