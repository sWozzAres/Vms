using System.Text.Json;
using System.Threading;
using Blazored.LocalStorage;

namespace Vms.Web.Client.Services;

public interface ISearchHistoryProvider
{
    //Task<IEnumerable<string>> GetHistory(CancellationToken cancellationToken = default);
    IEnumerable<string> History { get; }
    Task AddAsync(string searchString, CancellationToken cancellationToken = default);
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class SearchHistoryProvider : ISearchHistoryProvider
{
    const int HistorySize = 5;
    const string Key = "History";

    readonly ILocalStorageService _localStorage;

    Queue<string> history { get; set; } = new();
    public IEnumerable<string> History => history;

    bool _isLoaded;

    public SearchHistoryProvider(ILocalStorageService localStorage) => _localStorage = localStorage;
    
    //public async Task<IEnumerable<string>> GetHistory(CancellationToken cancellationToken = default)
    //{
    //    await EnsureLoaded(cancellationToken);
    //    return history;
    //}

    async Task EnsureLoaded(CancellationToken cancellationToken)
    {
        if (!_isLoaded)
        {
            history = await _localStorage.GetItemAsync<Queue<string>>(Key, cancellationToken) ?? new(HistorySize);
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

        if (history.Count == HistorySize)
            _ = history.Dequeue();

        history.Enqueue(searchString);

        await _localStorage.SetItemAsync(Key, history, cancellationToken);
    }
}
