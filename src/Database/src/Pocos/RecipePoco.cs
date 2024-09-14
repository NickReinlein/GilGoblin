using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GilGoblin.Database.Pocos;

public class RecipePoco
{
    [JsonPropertyName("id")] [Column("id")] public int Id { get; set; }

    [JsonPropertyName("craft_type")] public int CraftType { get; set; }

    [JsonPropertyName("recipe_level_table")] public int RecipeLevelTable { get; set; }

    [JsonPropertyName("target_item_id")] public int TargetItemId { get; set; }

    [JsonPropertyName("result_quantity")] public int ResultQuantity { get; set; }

    [JsonPropertyName("can_hq")] public bool CanHq { get; set; }

    [JsonPropertyName("can_quick_synth")] public bool CanQuickSynth { get; set; }

    [JsonPropertyName("item_ingredient0_target_id")]
    public int ItemIngredient0TargetId { get; set; }

    [JsonPropertyName("amount_ingredient0")]
    public int AmountIngredient0 { get; set; }

    [JsonPropertyName("item_ingredient1_target_id")]
    public int ItemIngredient1TargetId { get; set; }

    [JsonPropertyName("amount_ingredient1")]
    public int AmountIngredient1 { get; set; }

    [JsonPropertyName("item_ingredient2_target_id")]
    public int ItemIngredient2TargetId { get; set; }

    [JsonPropertyName("amount_ingredient2")]
    public int AmountIngredient2 { get; set; }

    [JsonPropertyName("item_ingredient3_target_id")]
    public int ItemIngredient3TargetId { get; set; }

    [JsonPropertyName("amount_ingredient3")]
    public int AmountIngredient3 { get; set; }

    [JsonPropertyName("item_ingredient4_target_id")]
    public int ItemIngredient4TargetId { get; set; }

    [JsonPropertyName("amount_ingredient4")]
    public int AmountIngredient4 { get; set; }

    [JsonPropertyName("item_ingredient5_target_id")]
    public int ItemIngredient5TargetId { get; set; }

    [JsonPropertyName("amount_ingredient5")]
    public int AmountIngredient5 { get; set; }

    [JsonPropertyName("item_ingredient6_target_id")]
    public int ItemIngredient6TargetId { get; set; }

    [JsonPropertyName("amount_ingredient6")]
    public int AmountIngredient6 { get; set; }

    [JsonPropertyName("item_ingredient7_target_id")]
    public int ItemIngredient7TargetId { get; set; }

    [JsonPropertyName("amount_ingredient7")]
    public int AmountIngredient7 { get; set; }

    [JsonPropertyName("item_ingredient8_target_id")]
    public int ItemIngredient8TargetId { get; set; }

    [JsonPropertyName("amount_ingredient8")]
    public int AmountIngredient8 { get; set; }

    [JsonPropertyName("item_ingredient9_target_id")]
    public int ItemIngredient9TargetId { get; set; }

    [JsonPropertyName("amount_ingredient9")]
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