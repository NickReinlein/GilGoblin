namespace GilGoblin.crafting
{
    public interface ICraftingCalculator {
        public int calculateCraftingCost(int itemID, int worldID, bool hq = false);
    }
}