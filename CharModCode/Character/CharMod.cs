using System;
using CharMod.CharModCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Scaffolding.Characters;

namespace CharMod.CharModCode.Character;

#pragma warning disable CS0618
public class CharMod : ModCharacterTemplate<CharModCardPool, CharModRelicPool, CharModPotionPool>
#pragma warning restore CS0618
{
    public const string CharacterId = "CharMod";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override int StartingGold => 99;
    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    [Obsolete("RitsuLib consumes this legacy template hook to build the sealed StartingDeck.")]
    protected override IEnumerable<Type> StartingDeckTypes => [
        typeof(SampleEnhanceStrike),
        typeof(StrikeIronclad),
        typeof(StrikeIronclad),
        typeof(StrikeIronclad),
        typeof(StrikeIronclad),
        typeof(DefendIronclad),
        typeof(DefendIronclad),
        typeof(DefendIronclad),
        typeof(DefendIronclad),
        typeof(DefendIronclad)
    ];

    [Obsolete("RitsuLib consumes this legacy template hook to build the sealed StartingRelics.")]
    protected override IEnumerable<Type> StartingRelicTypes =>
    [
        typeof(BurningBlood)
    ];

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];

    /*  PlaceholderCharacterModel will use basegame placeholder UI assets until a real mod package provides custom
        character art. Keeping these defaults avoids headless test failures from unloaded template PNG resources. */
}
