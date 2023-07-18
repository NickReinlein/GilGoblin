namespace GilGoblin.Pocos;

public class IngredientPoco
{
    public int RecipeID { get; set; }
    public int ItemID { get; set; }
    public int Quantity { get; set; }

    public IngredientPoco(int itemID, int quantity, int recipeID)
    {
        ItemID = itemID;
        Quantity = quantity;
        RecipeID = recipeID;
    }

    public IngredientPoco(IngredientPoco copyMe)
    {
        RecipeID = copyMe.RecipeID;
        ItemID = copyMe.ItemID;
        Quantity = copyMe.Quantity;
    }
}
