using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public class ItemInfoFetcher : DataFetcher<ItemInfoWebPoco, ItemInfoWebResponse>, IItemInfoFetcher
{
    private readonly IMarketableItemIdsFetcher _marketableFetcher;
    private readonly IItemRepository _repo;
    private readonly ILogger<ItemInfoFetcher> _logger;

    public ItemInfoFetcher(
        IItemRepository repo,
        IMarketableItemIdsFetcher fetcher,
        ILogger<ItemInfoFetcher> logger,
        HttpClient? client = null)
        : base(BaseUrl, logger, client)
    {
        _repo = repo;
        _marketableFetcher = fetcher;
        _logger = logger;
    }

    protected override string GetUrlPathFromEntries(IEnumerable<int> ids, int? worldId = null)
    {
        var sb = new StringBuilder();
        sb.Append(BaseUrl);

        sb.Append(ids.First());

        sb.Append(ColumnsSuffix);

        return sb.ToString();
    }

    public async Task<List<List<int>>> GetIdsAsBatchJobsAsync()
    {
        var missingIds = await GetAllMissingIds();
        if (!missingIds.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(ItemInfosPerPage);
        return batcher.SplitIntoBatchJobs(missingIds);
    }

    private async Task<List<int>> GetAllMissingIds()
    {
        var allIds = await _marketableFetcher.GetMarketableItemIdsAsync();
        if (!allIds.Any())
            return new List<int>();

        var existingIds = _repo.GetAll().Select(i => i.Id).ToList();
        return allIds.Except(existingIds).ToList();
    }

    public int ItemInfosPerPage { get; set; } = 1;

    public static readonly string BaseUrl = "https://xivapi.com/item";

    public static readonly string ColumnsSuffix =
        "?columns=ID,Name,Description,IconID,PriceMid,PriceLow,StackSize,LevelItem,CanBeHq";
}