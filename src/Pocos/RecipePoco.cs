using Newtonsoft.Json;

namespace GilGoblin.Pocos;

public class RecipePoco
{
    public int RecipeID { get; set; }
    public int TargetItemID { get; set; }
    public int IconID { get; set; }
    public int ResultQuantity { get; set; }
    public bool CanHq { get; set; }
    public bool CanQuickSynth { get; set; }
    public List<IngredientPoco> Ingredients { get; set; } = new List<IngredientPoco>();
    public RecipePoco() { }

    [JsonConstructor]
    public RecipePoco(bool CanQuickSynth, bool CanHq, int ItemResultTargetID, int ID, int IconID, int AmountResult,
        int AmountIngredient0, int AmountIngredient1,
        int AmountIngredient2,
        int AmountIngredient3,
        int AmountIngredient4,
        int AmountIngredient5,
        int AmountIngredient6,
        int AmountIngredient7,
        int AmountIngredient8,
        int AmountIngredient9,
        int ItemIngredient0TargetID,
        int ItemIngredient1TargetID,
        int ItemIngredient2TargetID,
        int ItemIngredient3TargetID,
        int ItemIngredient4TargetID,
        int ItemIngredient5TargetID,
        int ItemIngredient6TargetID,
        int ItemIngredient7TargetID,
        int ItemIngredient8TargetID,
        int ItemIngredient9TargetID) : base()
    {
        this.CanHq = CanHq;
        this.IconID = IconID;
        this.TargetItemID = ItemResultTargetID;
        this.RecipeID = ID;
        this.ResultQuantity = AmountResult;
        this.CanQuickSynth = CanQuickSynth;
        if (AmountIngredient0 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient0TargetID, AmountIngredient0, RecipeID));
        }
        if (AmountIngredient1 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient1TargetID, AmountIngredient1, this.RecipeID));
        }
        if (AmountIngredient2 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient2TargetID, AmountIngredient2, this.RecipeID));
        }
        if (AmountIngredient3 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient3TargetID, AmountIngredient3, this.RecipeID));
        }
        if (AmountIngredient4 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient4TargetID, AmountIngredient4, this.RecipeID));
        }
        if (AmountIngredient5 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient5TargetID, AmountIngredient5, this.RecipeID));
        }
        if (AmountIngredient6 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient6TargetID, AmountIngredient6, this.RecipeID));
        }
        if (AmountIngredient7 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient7TargetID, AmountIngredient7, this.RecipeID));
        }
        if (AmountIngredient8 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient8TargetID, AmountIngredient8, this.RecipeID));
        }
        if (AmountIngredient9 > 0)
        {
            Ingredients.Add(new IngredientPoco(ItemIngredient9TargetID, AmountIngredient9, this.RecipeID));
        }
    }

    public RecipePoco(RecipePoco old)
    {
        this.RecipeID = old.RecipeID;
        this.TargetItemID = old.TargetItemID;
        this.IconID = old.IconID;
        this.ResultQuantity = old.ResultQuantity;
        this.CanHq = old.CanHq;
        this.CanQuickSynth = old.CanQuickSynth;
        this.Ingredients = old.Ingredients;
    }
}