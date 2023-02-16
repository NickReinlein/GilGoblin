namespace GilGoblin.Pocos;

public class IngredientPoco
{
    public int RecipeID { get; set; }
    public int ItemID { get; set; }
    public int Quantity { get; set; }

    public IngredientPoco(int itemID, int quantity, int recipeID)
    {
        this.ItemID = itemID;
        this.Quantity = quantity;
        this.RecipeID = recipeID;
    }

    public IngredientPoco(IngredientPoco copyMe)
    {
        this.RecipeID = copyMe.RecipeID;
        this.ItemID = copyMe.ItemID;
        this.Quantity = copyMe.Quantity;
    }
}
