namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int ItemId,
    bool IsHq,
    decimal? World,
    decimal? Dc,
    decimal? Region)
    : IdentifiablePoco;