namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int ItemId,
    bool IsHq,
    SaleQuantity? World,
    SaleQuantity? Dc,
    SaleQuantity? Region)
    : IdentifiablePoco;