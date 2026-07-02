using System;
using CharMod.RitsuAdapters;
using CharMod.CharModCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using CharModCharacter = CharMod.CharModCode.Character.CharMod;

namespace CharMod.CharModCode.Cards;

public abstract class CharModCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ModComponentsCardTemplate(cost, type, rarity, target)
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

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{ImageStem}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{ImageStem}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{ImageStem}.png".CardImagePath();
}
