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

public class ItemInfoSingleFetcher : SingleDataFetcher<ItemInfoWebPoco>, IItemInfoSingleFetcher
{
    private readonly IMarketableItemIdsFetcher _marketableFetcher;
    private readonly IItemRepository _repo;
    private readonly ILogger<ItemInfoSingleFetcher> _logger;

    public ItemInfoSingleFetcher(
        IItemRepository repo,
        IMarketableItemIdsFetcher fetcher,
        ILogger<ItemInfoSingleFetcher> logger,
        HttpClient? client = null)
        : base(BaseUrl, logger, client)
    {
        _repo = repo;
        _marketableFetcher = fetcher;
        _logger = logger;
    }

    protected override string GetUrlPathFromEntry(int id, int? worldId = null)
    {
        var sb = new StringBuilder();
        sb.Append(BaseUrl);
        sb.Append(id);
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