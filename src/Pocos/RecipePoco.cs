using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace GilGoblin.Pocos;

public class RecipePoco
{
    [Name("#")]
    public int ID { get; set; }

    [Name("Item{Result}")]
    public int TargetItemID { get; set; }

    [Name("Amount{Result}")]
    public int ResultQuantity { get; set; }
    public bool CanHq { get; set; }
    public bool CanQuickSynth { get; set; }

    [Name("Amount{Ingredient}[0]")]
    public int AmountIngredient0 { get; set; }

    [Name("Amount{Ingredient}[1]")]
    public int AmountIngredient1 { get; set; }

    [Name("Amount{Ingredient}[2]")]
    public int AmountIngredient2 { get; set; }

    [Name("Amount{Ingredient}[3]")]
    public int AmountIngredient3 { get; set; }

    [Name("Amount{Ingredient}[4]")]
    public int AmountIngredient4 { get; set; }

    [Name("Amount{Ingredient}[5]")]
    public int AmountIngredient5 { get; set; }

    [Name("Amount{Ingredient}[6]")]
    public int AmountIngredient6 { get; set; }

    [Name("Amount{Ingredient}[7]")]
    public int AmountIngredient7 { get; set; }

    [Name("Amount{Ingredient}[8]")]
    public int AmountIngredient8 { get; set; }

    [Name("Amount{Ingredient}[9]")]
    public int AmountIngredient9 { get; set; }

    [Name("Item{Ingredient}[0]")]
    public int ItemIngredient0TargetID { get; set; }

    [Name("Item{Ingredient}[1]")]
    public int ItemIngredient1TargetID { get; set; }

    [Name("Item{Ingredient}[2]")]
    public int ItemIngredient2TargetID { get; set; }

    [Name("Item{Ingredient}[3]")]
    public int ItemIngredient3TargetID { get; set; }

    [Name("Item{Ingredient}[4]")]
    public int ItemIngredient4TargetID { get; set; }

    [Name("Item{Ingredient}[5]")]
    public int ItemIngredient5TargetID { get; set; }

    [Name("Item{Ingredient}[6]")]
    public int ItemIngredient6TargetID { get; set; }

    [Name("Item{Ingredient}[7]")]
    public int ItemIngredient7TargetID { get; set; }

    [Name("Item{Ingredient}[8]")]
    public int ItemIngredient8TargetID { get; set; }

    [Name("Item{Ingredient}[9]")]
    public int ItemIngredient9TargetID { get; set; }

    public RecipePoco() { }

    [JsonConstructor]
    public RecipePoco(
        bool canQuickSynth,
        bool canHq,
        int itemResultTargetID,
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
        int itemIngredient0TargetID,
        int itemIngredient1TargetID,
        int itemIngredient2TargetID,
        int itemIngredient3TargetID,
        int itemIngredient4TargetID,
        int itemIngredient5TargetID,
        int itemIngredient6TargetID,
        int itemIngredient7TargetID,
        int itemIngredient8TargetID,
        int itemIngredient9TargetID
    ) : base()
    {
        CanHq = canHq;
        TargetItemID = itemResultTargetID;
        ID = id;
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
        ItemIngredient0TargetID = itemIngredient0TargetID;
        ItemIngredient1TargetID = itemIngredient1TargetID;
        ItemIngredient2TargetID = itemIngredient2TargetID;
        ItemIngredient3TargetID = itemIngredient3TargetID;
        ItemIngredient4TargetID = itemIngredient4TargetID;
        ItemIngredient5TargetID = itemIngredient5TargetID;
        ItemIngredient6TargetID = itemIngredient6TargetID;
        ItemIngredient7TargetID = itemIngredient7TargetID;
        ItemIngredient8TargetID = itemIngredient8TargetID;
        ItemIngredient9TargetID = itemIngredient9TargetID;
        CanQuickSynth = canQuickSynth;
    }
}
