using CharMod.CharModCode.Extensions;
using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace CharMod.CharModCode.Character;

public class CharModRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "ironclad";
    public override Color LabOutlineColor => CharMod.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
