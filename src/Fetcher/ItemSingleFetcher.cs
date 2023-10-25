using GilGoblin.Pocos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using GilGoblin.Repository;

namespace GilGoblin.Fetcher;

public interface IItemSingleFetcher : ISingleDataFetcher<ItemWebPoco>
{
    Task<List<int>> GetAllMissingIds();
}

public class ItemSingleFetcher : SingleDataFetcher<ItemWebPoco>, IItemSingleFetcher
{
    private readonly IMarketableItemIdsFetcher _marketableFetcher;
    private readonly IItemRepository _repo;

    public ItemSingleFetcher(
        IItemRepository repo,
        IMarketableItemIdsFetcher fetcher,
        ILogger<ItemSingleFetcher> logger,
        HttpClient? client = null)
        : base(BaseUrl, logger, client)
    {
        _repo = repo;
        _marketableFetcher = fetcher;
    }

    protected override string GetUrlPathFromEntry(int id, int? worldId = null)
    {
        var sb = new StringBuilder();
        sb.Append(BaseUrl);
        sb.Append(id);
        sb.Append(ColumnsSuffix);
        return sb.ToString();
    }

    public async Task<List<int>> GetAllMissingIds()
    {
        var allIds = await _marketableFetcher.GetMarketableItemIdsAsync();
        if (!allIds.Any())
            return new List<int>();

        var existingIds = _repo.GetAll().Select(i => i.Id).ToList();
        return allIds.Except(existingIds).ToList();
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

    public static JsonSerializerOptions GetSerializerOptions()
        => new()
        {
            Converters = { new ItemWebPocoConverter() }, PropertyNameCaseInsensitive = true, IncludeFields = true,
        };
}