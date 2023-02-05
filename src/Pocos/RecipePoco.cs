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
    public List<IngredientPoco> Ingredients { get; set; } = new List<IngredientPoco>();

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
        this.CanHq = canHq;
        this.TargetItemID = itemResultTargetID;
        this.ID = id;
        this.ResultQuantity = amountResult;
        this.CanQuickSynth = canQuickSynth;
        if (amountIngredient0 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient0TargetID, amountIngredient0, ID));
        }
        if (amountIngredient1 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient1TargetID, amountIngredient1, ID));
        }
        if (amountIngredient2 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient2TargetID, amountIngredient2, ID));
        }
        if (amountIngredient3 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient3TargetID, amountIngredient3, ID));
        }
        if (amountIngredient4 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient4TargetID, amountIngredient4, ID));
        }
        if (amountIngredient5 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient5TargetID, amountIngredient5, ID));
        }
        if (amountIngredient6 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient6TargetID, amountIngredient6, ID));
        }
        if (amountIngredient7 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient7TargetID, amountIngredient7, ID));
        }
        if (amountIngredient8 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient8TargetID, amountIngredient8, ID));
        }
        if (amountIngredient9 > 0)
        {
            Ingredients.Add(new IngredientPoco(itemIngredient9TargetID, amountIngredient9, ID));
        }
    }

    public RecipePoco(RecipePoco old)
    {
        this.ID = old.ID;
        this.TargetItemID = old.TargetItemID;
        this.ResultQuantity = old.ResultQuantity;
        this.CanHq = old.CanHq;
        this.CanQuickSynth = old.CanQuickSynth;
        this.Ingredients = old.Ingredients;
    }
}
