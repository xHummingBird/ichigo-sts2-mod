using BaseLib.Abstracts;
using BaseLib.Utils;
using Ichigo.IchigoCode.Character;

namespace Ichigo.IchigoCode.Potions;

[Pool(typeof(IchigoPotionPool))]
public abstract class IchigoPotion : CustomPotionModel;