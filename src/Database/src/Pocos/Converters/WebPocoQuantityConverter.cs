using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GilGoblin.Database.Pocos.Converters;

public class WebPocoQuantityConverter() : ValueConverter<WebPocoQuantity?, decimal>(
    quantity => quantity.Quantity ?? 0,
    value => new WebPocoQuantity(value));