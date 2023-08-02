using System.Text.Json;
using System.Threading;
using Blazored.LocalStorage;

namespace Vms.Web.Client.Services;

public interface ISearchHistoryProvider
{
    //Task<IEnumerable<string>> GetHistory(CancellationToken cancellationToken = default);
    IEnumerable<SearchItem> History { get; }
    Task AddAsync(string searchString, CancellationToken cancellationToken = default);
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class SearchHistoryProvider(ILocalStorageService localStorage) : ISearchHistoryProvider
{
    const int HistorySize = 5;
    const string Key = "History";

    Queue<SearchItem> _history = new(HistorySize);
    public IEnumerable<SearchItem> History => _history;

    bool _isLoaded;

    async Task EnsureLoaded(CancellationToken cancellationToken)
    {
        if (!_isLoaded)
        {
            _history = await localStorage.GetItemAsync<Queue<SearchItem>>(Key, cancellationToken) ?? new(HistorySize);
            _isLoaded = true;
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await EnsureLoaded(cancellationToken);
    }

    public async Task AddAsync(string searchString, CancellationToken cancellationToken = default)
    {
        await EnsureLoaded(cancellationToken);

        var entry = _history.FirstOrDefault(item => item.SearchString == searchString);
        if (entry is not null)
        {
            entry.Count++;
        }
        else
        {
            if (_history.Count >= HistorySize)
                _ = _history.Dequeue();

            _history.Enqueue(new SearchItem() { SearchString = searchString, Count = 1 });
        }

        await localStorage.SetItemAsync(Key, _history, cancellationToken);
    }
}

public class SearchItem
{
    public string SearchString { get; set; } = null!;
    public int Count { get; set; }
}
