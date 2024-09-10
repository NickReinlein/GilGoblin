using System;

namespace GilGoblin.Database.Pocos;

public record PricePoco : IIdentifiable
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int WorldId { get; set; }
    public bool IsHq { get; set; }
    public int? MinListingId { get; set; }
    public int? RecentPurchaseId { get; set; }
    public int? AverageSalePriceId { get; set; }
    public int? DailySaleVelocityId { get; set; }
    public DateTimeOffset Updated { get; set; }
    public int GetId() => Id;
}