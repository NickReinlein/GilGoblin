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
    public int AmountIngredient0 { get; set; }
    public int AmountIngredient1 { get; set; }
    public int AmountIngredient2 { get; set; }
    public int AmountIngredient3 { get; set; }
    public int AmountIngredient4 { get; set; }
    public int AmountIngredient5 { get; set; }
    public int AmountIngredient6 { get; set; }
    public int AmountIngredient7 { get; set; }
    public int AmountIngredient8 { get; set; }
    public int AmountIngredient9 { get; set; }
    public int ItemIngredient0TargetID { get; set; }
    public int ItemIngredient1TargetID { get; set; }
    public int ItemIngredient2TargetID { get; set; }
    public int ItemIngredient3TargetID { get; set; }
    public int ItemIngredient4TargetID { get; set; }
    public int ItemIngredient5TargetID { get; set; }
    public int ItemIngredient6TargetID { get; set; }
    public int ItemIngredient7TargetID { get; set; }
    public int ItemIngredient8TargetID { get; set; }
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

    public List<IngredientPoco> Ingredients =>
        new()
        {
            new IngredientPoco(ItemIngredient0TargetID, AmountIngredient1, ID),
            new IngredientPoco(ItemIngredient1TargetID, AmountIngredient2, ID),
            new IngredientPoco(ItemIngredient2TargetID, AmountIngredient3, ID),
            new IngredientPoco(ItemIngredient3TargetID, AmountIngredient4, ID),
            new IngredientPoco(ItemIngredient5TargetID, AmountIngredient5, ID),
            new IngredientPoco(ItemIngredient6TargetID, AmountIngredient6, ID),
            new IngredientPoco(ItemIngredient7TargetID, AmountIngredient7, ID),
            new IngredientPoco(ItemIngredient8TargetID, AmountIngredient8, ID),
            new IngredientPoco(ItemIngredient9TargetID, AmountIngredient9, ID)
        };
}
