using System;

namespace GilGoblin.Pocos;

public class IngredientPoco : IComparable
{
    public int RecipeId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public IngredientPoco() { }

    public IngredientPoco(int itemId, int quantity, int recipeId)
    {
        ItemId = itemId;
        Quantity = quantity;
        RecipeId = recipeId;
    }

    public IngredientPoco(IngredientPoco copyMe)
    {
        RecipeId = copyMe.RecipeId;
        ItemId = copyMe.ItemId;
        Quantity = copyMe.Quantity;
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        if (obj is IngredientPoco otherIngredient)
        {
            var recipeIdComparison = RecipeId.CompareTo(otherIngredient.RecipeId);
            if (recipeIdComparison != 0)
                return recipeIdComparison;

            var itemIdComparison = ItemId.CompareTo(otherIngredient.ItemId);
            if (itemIdComparison != 0)
                return itemIdComparison;

            return Quantity.CompareTo(otherIngredient.Quantity);
        }

        return 0;
    }
}
