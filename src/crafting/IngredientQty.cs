namespace GilGoblin.crafting
{
    public class IngredientQty
    {
        public IngredientQty() { }

        public IngredientQty(int id, int qty)
        {
            ID = id;
            this.qty = qty;
        }

        public int ID { get; set; }
        public int qty { get; set; }
        
    }
}