using System;

namespace GilGoblin.Pocos;

public class IngredientPoco : IComparable
{
    public int RecipeID { get; set; }
    public int ItemID { get; set; }
    public int Quantity { get; set; }

    public IngredientPoco() { }

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

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        if (obj is IngredientPoco otherIngredient)
        {
            var recipeIdComparison = RecipeID.CompareTo(otherIngredient.RecipeID);
            if (recipeIdComparison != 0)
                return recipeIdComparison;

            var itemIdComparison = ItemID.CompareTo(otherIngredient.ItemID);
            if (itemIdComparison != 0)
                return itemIdComparison;

            return Quantity.CompareTo(otherIngredient.Quantity);
        }

        return 0;
    }
}
