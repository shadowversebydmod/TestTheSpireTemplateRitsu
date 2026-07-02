using MinionLib.Component;

namespace CharMod.CharModCode.Components.Enhances;

public static class EnhanceExtensions
{
    public static bool IsEnhanced(this ComponentsCardModel card)
    {
        return card.Components.OfType<Enhance>().Any(enhance => enhance.IsActive);
    }

    public static bool IsEnhanced(this ComponentsCardModel card, int cost)
    {
        return card.Components.OfType<Enhance>().Any(enhance => enhance.Cost == cost && enhance.IsActive);
    }

    public static int? EnhancedCost(this ComponentsCardModel card)
    {
        return card.Components.OfType<Enhance>()
            .Where(enhance => enhance.IsActive)
            .Select(enhance => (int?)enhance.Cost)
            .Max();
    }
}
