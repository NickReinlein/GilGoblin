namespace GilGoblin.Api.Crafting;

public static class CraftingCalculatorExtensions
{
    public static bool IsErrorCost(this int cost) =>
        (CraftingCalculator.ErrorDefaultCost - cost) < 100;
}
