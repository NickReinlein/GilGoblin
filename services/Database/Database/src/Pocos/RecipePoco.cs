namespace GilGoblin.Database.Pocos;

public class RecipePoco
{
    public int Id { get; set; }
    public int CraftType { get; set; }
    public int RecipeLevelTable { get; set; }
    public int TargetItemId { get; set; }
    public int ResultQuantity { get; set; }
    public bool CanHq { get; set; }
    public bool CanQuickSynth { get; set; }
    public int ItemIngredient0TargetId { get; set; }
    public int AmountIngredient0 { get; set; }
    public int ItemIngredient1TargetId { get; set; }
    public int AmountIngredient1 { get; set; }
    public int ItemIngredient2TargetId { get; set; }
    public int AmountIngredient2 { get; set; }
    public int ItemIngredient3TargetId { get; set; }
    public int AmountIngredient3 { get; set; }
    public int ItemIngredient4TargetId { get; set; }
    public int AmountIngredient4 { get; set; }
    public int ItemIngredient5TargetId { get; set; }
    public int AmountIngredient5 { get; set; }
    public int ItemIngredient6TargetId { get; set; }
    public int AmountIngredient6 { get; set; }
    public int ItemIngredient7TargetId { get; set; }
    public int AmountIngredient7 { get; set; }
    public int ItemIngredient8TargetId { get; set; }
    public int AmountIngredient8 { get; set; }
    public int ItemIngredient9TargetId { get; set; }
    public int AmountIngredient9 { get; set; }


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
        Id = id;
        CanHq = canHq;
        TargetItemId = itemResultTargetId;
        CanQuickSynth = canQuickSynth;
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
    }
}