namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int ItemId,
    int WorldId,
    bool IsHq,
    SaleQuantity? World,
    SaleQuantity? Dc,
    SaleQuantity? Region)
    : IdentifiableTripleKeyPoco(ItemId, WorldId, IsHq);