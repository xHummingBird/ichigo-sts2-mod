using BaseLib.Abstracts;
using Ichigo.IchigoCode.Extensions;
using Godot;

namespace Ichigo.IchigoCode.Character;

public class IchigoPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Ichigo.Color;
    

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}