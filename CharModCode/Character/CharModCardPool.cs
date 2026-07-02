using CharMod.CharModCode.Extensions;
using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace CharMod.CharModCode.Character;

public class CharModCardPool : TypeListCardPoolModel
{
    public override string Title => CharMod.CharacterId; //This is not a display name.

    public override string EnergyColorName => "ironclad";
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}
