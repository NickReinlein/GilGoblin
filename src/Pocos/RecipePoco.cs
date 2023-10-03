using CsvHelper.Configuration.Attributes;

namespace GilGoblin.Pocos;

public class RecipePoco
{
    [Name("#")] public int Id { get; set; }

    [Name("Item{Result}")] public int TargetItemId { get; set; }

    [Name("Amount{Result}")] public int ResultQuantity { get; set; }
    public bool CanHq { get; set; }
    public bool CanQuickSynth { get; set; }

    [Name("Amount{Ingredient}[0]")] public int AmountIngredient0 { get; set; }

    [Name("Amount{Ingredient}[1]")] public int AmountIngredient1 { get; set; }

    [Name("Amount{Ingredient}[2]")] public int AmountIngredient2 { get; set; }

    [Name("Amount{Ingredient}[3]")] public int AmountIngredient3 { get; set; }

    [Name("Amount{Ingredient}[4]")] public int AmountIngredient4 { get; set; }

    [Name("Amount{Ingredient}[5]")] public int AmountIngredient5 { get; set; }

    [Name("Amount{Ingredient}[6]")] public int AmountIngredient6 { get; set; }

    [Name("Amount{Ingredient}[7]")] public int AmountIngredient7 { get; set; }

    [Name("Amount{Ingredient}[8]")] public int AmountIngredient8 { get; set; }

    [Name("Amount{Ingredient}[9]")] public int AmountIngredient9 { get; set; }

    [Name("Item{Ingredient}[0]")] public int ItemIngredient0TargetId { get; set; }

    [Name("Item{Ingredient}[1]")] public int ItemIngredient1TargetId { get; set; }

    [Name("Item{Ingredient}[2]")] public int ItemIngredient2TargetId { get; set; }

    [Name("Item{Ingredient}[3]")] public int ItemIngredient3TargetId { get; set; }

    [Name("Item{Ingredient}[4]")] public int ItemIngredient4TargetId { get; set; }

    [Name("Item{Ingredient}[5]")] public int ItemIngredient5TargetId { get; set; }

    [Name("Item{Ingredient}[6]")] public int ItemIngredient6TargetId { get; set; }

    [Name("Item{Ingredient}[7]")] public int ItemIngredient7TargetId { get; set; }

    [Name("Item{Ingredient}[8]")] public int ItemIngredient8TargetId { get; set; }

    [Name("Item{Ingredient}[9]")] public int ItemIngredient9TargetId { get; set; }

    public RecipePoco() { }

    public RecipePoco(
        bool canQuickSynth,
        bool canHq,
        int itemResultTargetId,
        int id,
        int amountResult,
        int amountIngredient0,
        int amountIngredient1,
        int amountIngredient2,
        int amountIngredient3,
        int amountIngredient4,
        int amountIngredient5,
        int amountIngredient6,
        int amountIngredient7,
        int amountIngredient8,
        int amountIngredient9,
        int itemIngredient0TargetId,
        int itemIngredient1TargetId,
        int itemIngredient2TargetId,
        int itemIngredient3TargetId,
        int itemIngredient4TargetId,
        int itemIngredient5TargetId,
        int itemIngredient6TargetId,
        int itemIngredient7TargetId,
        int itemIngredient8TargetId,
        int itemIngredient9TargetId
    )
    {
        CanHq = canHq;
        TargetItemId = itemResultTargetId;
        Id = id;
        ResultQuantity = amountResult;
        AmountIngredient0 = amountIngredient0;
        AmountIngredient1 = amountIngredient1;
        AmountIngredient2 = amountIngredient2;
        AmountIngredient3 = amountIngredient3;
        AmountIngredient4 = amountIngredient4;
        AmountIngredient5 = amountIngredient5;
        AmountIngredient6 = amountIngredient6;
        AmountIngredient7 = amountIngredient7;
        AmountIngredient8 = amountIngredient8;
        AmountIngredient9 = amountIngredient9;
        ItemIngredient0TargetId = itemIngredient0TargetId;
        ItemIngredient1TargetId = itemIngredient1TargetId;
        ItemIngredient2TargetId = itemIngredient2TargetId;
        ItemIngredient3TargetId = itemIngredient3TargetId;
        ItemIngredient4TargetId = itemIngredient4TargetId;
        ItemIngredient5TargetId = itemIngredient5TargetId;
        ItemIngredient6TargetId = itemIngredient6TargetId;
        ItemIngredient7TargetId = itemIngredient7TargetId;
        ItemIngredient8TargetId = itemIngredient8TargetId;
        ItemIngredient9TargetId = itemIngredient9TargetId;
        CanQuickSynth = canQuickSynth;
    }
}