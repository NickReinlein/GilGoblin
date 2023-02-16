using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using GilGoblin.Pocos;

namespace GilGoblin.Extension;

public static class RecipePocoExtension
{
    public static List<IngredientPoco> GetIngredientsList(this RecipePoco poco) =>
        new()
        {
            new IngredientPoco(poco.ItemIngredient0TargetID, poco.AmountIngredient0, poco.ID),
            new IngredientPoco(poco.ItemIngredient1TargetID, poco.AmountIngredient1, poco.ID),
            new IngredientPoco(poco.ItemIngredient2TargetID, poco.AmountIngredient2, poco.ID),
            new IngredientPoco(poco.ItemIngredient3TargetID, poco.AmountIngredient3, poco.ID),
            new IngredientPoco(poco.ItemIngredient4TargetID, poco.AmountIngredient4, poco.ID),
            new IngredientPoco(poco.ItemIngredient5TargetID, poco.AmountIngredient5, poco.ID),
            new IngredientPoco(poco.ItemIngredient6TargetID, poco.AmountIngredient6, poco.ID),
            new IngredientPoco(poco.ItemIngredient7TargetID, poco.AmountIngredient7, poco.ID),
            new IngredientPoco(poco.ItemIngredient8TargetID, poco.AmountIngredient8, poco.ID),
            new IngredientPoco(poco.ItemIngredient9TargetID, poco.AmountIngredient9, poco.ID)
        };

    public static List<IngredientPoco> GetActiveIngredients(this RecipePoco poco) =>
        poco.GetIngredientsList().Where(i => i != null && i.Quantity > 0).ToList();
}
