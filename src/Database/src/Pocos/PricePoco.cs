using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Database.Pocos;

public record PricePoco(
    int ItemId,
    int WorldId,
    bool IsHq,
    DateTimeOffset Updated,
    MinListingPoco? MinListing = null,
    RecentPurchasePoco? RecentPurchase = null,
    AverageSalePricePoco? AverageSalePrice = null,
    DailySaleVelocityPoco? DailySaleVelocity = null)
    : IIdentifiable
{
    [Column("id")] public int Id { get; set; }
    public int ItemId { get; set; } = ItemId;
    public int WorldId { get; set; } = WorldId;
    public bool IsHq { get; set; } = IsHq;
    public int? MinListingId { get; set; } = MinListing?.Id;
    public int? RecentPurchaseId { get; set; } = RecentPurchase?.Id;
    public int? AverageSalePriceId { get; set; } = AverageSalePrice?.Id;
    public int? DailySaleVelocityId { get; set; } = DailySaleVelocity?.Id;
    public DateTimeOffset Updated { get; set; } = Updated;
    public MinListingPoco? MinListing { get; set; } = MinListing;
    public RecentPurchasePoco? RecentPurchase { get; set; } = RecentPurchase;
    public AverageSalePricePoco? AverageSalePrice { get; set; } = AverageSalePrice;
    public DailySaleVelocityPoco? DailySaleVelocity { get; set; } = DailySaleVelocity;

    public int GetId() => Id;
}