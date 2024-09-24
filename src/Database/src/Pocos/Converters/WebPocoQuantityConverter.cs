using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GilGoblin.Database.Pocos.Converters;

public class WebPocoQuantityConverter() : ValueConverter<WebPocoQuantity?, float>(
    quantity => quantity.Quantity.HasValue ? quantity.Quantity.Value : 0,
    value => new WebPocoQuantity(value));