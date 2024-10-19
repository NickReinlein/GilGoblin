using System;

namespace GilGoblin.Database.Pocos;

public class IngredientPoco : IComparable
{
    public int RecipeId { get; init; }
    public int ItemId { get; init; }
    public int Quantity { get; init; }
    public bool IsHq { get; init; }

    public IngredientPoco() { }

    public IngredientPoco(int itemId, int quantity, int recipeId, bool isHq)
    {
        ItemId = itemId;
        Quantity = quantity;
        RecipeId = recipeId;
        IsHq = isHq;
    }

    public IngredientPoco(IngredientPoco copyMe)
    {
        RecipeId = copyMe.RecipeId;
        ItemId = copyMe.ItemId;
        Quantity = copyMe.Quantity;
        IsHq = copyMe.IsHq;
    }

    public int CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        if (obj is IngredientPoco otherIngredient)
        {
            if (ItemId != otherIngredient.ItemId)
                return ItemId.CompareTo(otherIngredient.ItemId);
            
            if (RecipeId != otherIngredient.RecipeId)
                return RecipeId.CompareTo(otherIngredient.RecipeId);
            
            if (IsHq != otherIngredient.IsHq)
                return IsHq.CompareTo(otherIngredient.IsHq);

            return Quantity.CompareTo(otherIngredient.Quantity);
        }

        return 0;
    }
}
