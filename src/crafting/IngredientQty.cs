namespace GilGoblin.crafting
{
    public class IngredientQty
    {
        public IngredientQty() { }

        public IngredientQty(int id, int qty)
        {
            this.ItemID = id;
            this.Quantity = qty;
        }

        public int ItemID { get; set; }
        public int Quantity { get; set; }
    }
}
