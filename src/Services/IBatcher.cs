namespace GilGoblin.Services;

public interface IBatcher<T>
{
    List<List<T>> SplitIntoBatchJobs(List<T> entries);
}
