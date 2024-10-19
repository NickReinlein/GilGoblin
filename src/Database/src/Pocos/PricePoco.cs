using System;

namespace GilGoblin.Database.Pocos;

public record PricePoco(
    int ItemId,
    int WorldId,
    bool IsHq,
    DateTimeOffset Updated,
    int? MinListingId = null,
    int? RecentPurchaseId = null,
    int? AverageSalePriceId = null,
    int? DailySaleVelocityId = null)
    : IdentifiablePoco
{
    // Navigation properties
    public MinListingPoco? MinListing { get; init; }
    public RecentPurchasePoco? RecentPurchase { get; init; }
    public AverageSalePricePoco? AverageSalePrice { get; init; }
    public DailySaleVelocityPoco? DailySaleVelocity { get; init; }
    
    public int? GetBestPrice =>
        RecentPurchase?. ?? AverageSalePrice?.Price ;
}