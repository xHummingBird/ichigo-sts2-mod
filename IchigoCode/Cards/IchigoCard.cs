using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Ichigo.IchigoCode.Character;
using Ichigo.IchigoCode.Extensions;
using Ichigo.IchigoCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization;

namespace Ichigo.IchigoCode.Cards;

[Pool(typeof(IchigoCardPool))]
public abstract class IchigoCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    // In True Bankai, HP loss from cards is converted into healing of the same amount.
    protected async Task LoseHpOrHeal(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal amount = DynamicVars.HpLoss.BaseValue;
        
        await CreatureCmd.Damage(choiceContext, Owner.Creature, amount,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        if (ReiryokuManager.IsInTrueBankai(Owner))
        {
            await CreatureCmd.Heal(Owner.Creature, amount);
        }
    }

    protected override void AddExtraArgsToDescription(
        LocString description)
    {
        base.AddExtraArgsToDescription(description);

        description.Add(
            "BankaiIcon",
            "[img]res://Ichigo/images/charui/bankai_icon.png[/img]");
    }
}
