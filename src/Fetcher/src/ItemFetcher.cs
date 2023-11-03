using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace GilGoblin.Fetcher;

public interface IItemFetcher : ISingleDataFetcher<ItemWebPoco>
{
}

public class ItemFetcher : SingleDataFetcher<ItemWebPoco>, IItemFetcher
{
    public ItemFetcher(ILogger<ItemFetcher> logger, HttpClient? client = null)
        : base(BaseUrl, logger, client)
    {
    }

    protected override string GetUrlPathFromEntry(int id, int? worldId = null)
    {
        var sb = new StringBuilder();
        sb.Append(BaseUrl);
        sb.Append(id);
        sb.Append(ColumnsSuffix);
        return sb.ToString();
    }

    public static readonly string BaseUrl = "https://xivapi.com/item/";

    public static readonly string ColumnsSuffix =
        "?columns=ID,Name,Description,IconID,PriceMid,PriceLow,StackSize,LevelItem,CanBeHq";

    protected override ItemWebPoco ReadResponseContentAsync(HttpContent content)
        => JsonSerializer.DeserializeAsync<ItemWebPoco>(
            content.ReadAsStream(),
            GetSerializerOptions(),
            CancellationToken.None
        ).Result;

    public static JsonSerializerOptions GetSerializerOptions() => new()
    {
        Converters = { new ItemWebPocoConverter() }, PropertyNameCaseInsensitive = true, IncludeFields = true,
    };
}