namespace GilGoblin.Api.Crafting;

public static class CraftingCalculatorExtensions
{
    public static bool IsErrorCost(this int cost) =>
        (CraftingCalculator.ERROR_DEFAULT_COST - cost) < 100;
}
