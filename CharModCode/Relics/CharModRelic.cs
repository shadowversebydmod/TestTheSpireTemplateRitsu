using System;
using CharMod.CharModCode.Extensions;
using STS2RitsuLib.Scaffolding.Content;
using CharModCharacter = CharMod.CharModCode.Character.CharMod;

namespace CharMod.CharModCode.Relics;

/// <summary>
/// This is the base class for your mod's relics, which is set up to load the relic's images from your mod's resources.
/// When creating a relic, right click the Relics folder and create a new file with the Custom Relic template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
///
/// The [Pool] annotation marks this relic as being tied to your specific character. Inheriting from this class means
/// that your relics don't need to invidually say which pool they should be in.
/// </summary>
public abstract class CharModRelic : ModRelicTemplate
{
    private string ImageStem
    {
        get
        {
            var prefix = $"{CharModCharacter.CharacterId}-";
            var entry = Id.Entry;
            if (entry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                entry = entry[prefix.Length..];
            return entry.ToLowerInvariant();
        }
    }

    public override string CustomIconPath => $"{ImageStem}.png".RelicImagePath();
    public override string CustomIconOutlinePath => $"{ImageStem}_outline.png".RelicImagePath();
    public override string CustomBigIconPath => $"{ImageStem}.png".BigRelicImagePath();
}
