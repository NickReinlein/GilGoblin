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
    public MinListingPoco? MinListing { get; set; }
    public RecentPurchasePoco? RecentPurchase { get; set; }
    public AverageSalePricePoco? AverageSalePrice { get; set; }
    public DailySaleVelocityPoco? DailySaleVelocity { get; set; }

    public PriceDataPoco? GetBestPrice() => AverageSalePrice?.GetBestPrice() ??
                                            RecentPurchase?.GetBestPrice() ??
                                            MinListing?.GetBestPrice();
    public decimal GetBestPriceCost() => GetBestPrice()?.Price ?? -1m;
}