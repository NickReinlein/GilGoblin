using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Database.Pocos;

public record PricePoco : IIdentifiable
{
    [Column("id")] public int Id { get; set; }
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public bool IsHq { get; set; }
    public int? MinListingId { get; set; }
    public int? RecentPurchaseId { get; set; }
    public int? AverageSalePriceId { get; set; }
    public int? DailySaleVelocityId { get; set; }
    public DateTimeOffset Updated { get; set; }
    public MinListingPoco? MinListing { get; set; }
    public RecentPurchasePoco? RecentPurchase { get; set; }
    public AverageSalePricePoco? AverageSalePrice { get; set; }
    public DailySaleVelocityPoco? DailySaleVelocity { get; set; }
    public int GetId() => Id;
}