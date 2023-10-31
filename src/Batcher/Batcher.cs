using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Batcher;

public class Batcher<T> : IBatcher<T>
{
    protected readonly int PageSize;

    public Batcher(int pageSize = 100)
    {
        PageSize = pageSize;
    }

    public List<List<T>> SplitIntoBatchJobs(List<T> entries)
    {
        if (!entries.Any() || PageSize < 1)
            return new List<List<T>>();

        var cumulativeList = new List<List<T>>();
        var tempList = new List<T>();
        var queue = new Queue<T>(entries);
        while (queue.Any())
        {
            var entry = queue.Dequeue();
            tempList.Add(entry);

            if (tempList.Count == PageSize)
            {
                cumulativeList.Add(tempList);
                tempList = new List<T>();
            }
        }

        if (tempList.Any())
            cumulativeList.Add(tempList);

        return cumulativeList;
    }
}