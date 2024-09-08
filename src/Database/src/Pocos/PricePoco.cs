using System;

namespace GilGoblin.Database.Pocos;

public record PricePoco : IIdentifiable
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public bool IsHq { get; set; }
    public MinListingPoco? MinListing { get; set; }
    public RecentPurchasePoco? RecentPurchase { get; set; }
    public AverageSalePricePoco? AverageSalePrice { get; set; }
    public DailySaleVelocityDbPoco? DailySaleVelocity { get; set; }
    public DateTimeOffset Updated { get; set; }
    public int GetId() => Id;
}