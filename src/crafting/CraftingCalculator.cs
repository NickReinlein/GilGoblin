namespace GilGoblin.crafting
{
    public class CraftingCalculator : ICraftingCalculator
    {
        public int calculateCraftingCost(int itemID, int worldID, bool hq = false)
        {
            return hq ? 2 : 1;
        }
    }
}