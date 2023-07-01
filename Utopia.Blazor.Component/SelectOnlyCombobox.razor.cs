using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Xml.Linq;
using Utopia.Blazor.Component.Helpers;

namespace Utopia.Blazor.Component;

public partial class SelectOnlyCombobox<TValue> : ComponentBase, IAsyncDisposable
{
    [Inject]
    public IJSRuntime JS { get; set; } = null!;
    [Inject]
    public ILogger<SelectOnlyCombobox<TValue>> Logger { get; set; } = null!;

    [EditorRequired, Parameter]
    public List<SelectOption<TValue>> Items { get; set; } = null!;

    [EditorRequired, Parameter]
    public string Label { get; set; } = null!;

    [Parameter]
    public bool LabelHidden { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> InputAttributes { get; set; } = null!;

    [CascadingParameter(Name = "IsDisabled")]
    public bool IsDisabled { get; set; }

    [Parameter]
    public TValue? SelectedValue { get; set; } = default;
    [Parameter]
    public EventCallback<TValue> SelectedValueChanged { get; set; }

    Dictionary<int, SelectItem> selectItems { get; set; } = new();

    string classNameAddition = "";
    string labelClassName => string.Join(" ", LabelHidden ? "combo-label hidden" : "combo-label", classNameAddition);
    string comboClassName => string.Join(" ", open ? "combo open" : "combo", classNameAddition);
    string comboInputClassName => IsDisabled ? "combo-input disabled" : "combo-input";
    string comboInputTabIndex => IsDisabled ? "-1" : "0";
    string activeItemId => open ? itemId(activeIndex) : string.Empty;
    string itemId(int i) => $"{id}-{i}";

    // ids
    string id = null!;
    string comboInputId = null!;
    string comboMenuId = null!;
    int selectedIndex;

    // element references
    ElementReference comboEl;
    ElementReference comboInputEl;
    ElementReference comboMenuEl;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _jsEventDisposable;

    DotNetObjectReference<SelectOnlyCombobox<TValue>>? objRef;

    // state
    int activeIndex;
    bool open;
    string searchString = string.Empty;
    bool ignoreBlur;

    Timer? timer;

    async Task updateMenuState(bool shouldOpen, bool callFocus = true)
    {
        if (shouldOpen == open)
            return;

        open = shouldOpen;

        if (callFocus)
            await comboInputEl.FocusAsync();
    }

    async Task selectOption(int index)
    {
        if (selectedIndex == index)
            return;

        selectedIndex = index;
        Logger.LogInformation("Selected index {index}.", index);

        SelectedValue = selectItems[selectedIndex].Value;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }

    #region ComboInput events
    async Task onComboInputBlur()
    {
        Logger.LogInformation("onComboInputBlur() {ignoreBlur}", ignoreBlur);
        if (ignoreBlur)
        {
            ignoreBlur = false;
            return;
        }

        if (open)
        {
            //await selectOption(activeIndex);
            await updateMenuState(false, false);
        }
    }

    async Task onComboInputClick()
    {
        if (IsDisabled)
            return;

        await updateMenuState(!open, false);
    }
    #endregion

    void onOptionChange(int index) => activeIndex = index;

    #region ComboMenu events
    async Task onComboMenuClick(int index)
    {
        Logger.LogInformation("OnComboMenuClick('{index}')", index);
        onOptionChange(index);

        await updateMenuState(false);
        await selectOption(index);
    }
    void onComboMenuMouseDown()
    {
        Logger.LogInformation("onComboMenuMouseDown()");
        ignoreBlur = true;
    }
    #endregion

    string getSearchString(string key)
    {
        Logger.LogInformation("getSearchString({key})", key);


        timer?.Dispose();

        timer = new System.Threading.Timer((state) =>
        {
            searchString = string.Empty;
            Logger.LogInformation("Timer tick.");
        }, null, TimeSpan.FromMilliseconds(5000), Timeout.InfiniteTimeSpan);

        if (key == "Backspace")
        {
            if (searchString.Length > 0)
                searchString = searchString[0..^1];
        }
        else if (key == "Clear")
        {
            searchString = string.Empty;
        }
        else if (key.Length == 1)
        {
            searchString = searchString + key[0];
        }

        Logger.LogInformation("SearchString '{searchString}'", searchString);

        return searchString;
    }

    async Task<string> onComboType(string key)
    {
        await updateMenuState(true);

        var searchString1 = getSearchString(key);
        var searchIndex = getIndexByLetter(searchString1, activeIndex + 1);
        if (searchIndex >= 0)
        {
            Logger.LogInformation("Found index {index}.", searchIndex);

            onOptionChange(searchIndex);

            return itemId(searchIndex);
        }
        else
        {
            timer?.Dispose();
            timer = null;

            searchString = string.Empty;

            return "";
        }

        int getIndexByLetter(string ss, int index)
        {
            var orderedOptions = selectItems.Where(item => item.Key >= index)
                .Concat(selectItems.Where(item => item.Key < index));

            Logger.LogInformation("orderedOptions {o}", orderedOptions.Select(x => x.Value.Name));

            var firstMatch = orderedOptions.Where(x => x.Value.Name.StartsWith(ss, StringComparison.OrdinalIgnoreCase));

            return firstMatch.Any() ? firstMatch.First().Key : -1;
        }
    }

    public enum SelectAction : int
    {
        Close = 0,
        CloseSelect = 1,
        First = 2,
        Last = 3,
        Next = 4,
        Open = 5,
        PageDown = 6,
        PageUp = 7,
        Previous = 8,
        Select = 9,
        Type = 10,
    };

    [JSInvokable]
    public async Task<string> onComboTypeJS(string key)
    {
        var result = await onComboType(key);
        StateHasChanged();
        return result;
    }

    [JSInvokable]
    public void onOptionChangeJS(int actionInt)
    {
        Logger.LogInformation("onOptionChangeJS({action})", actionInt);

        SelectAction action = (SelectAction)actionInt;
        onOptionChange(getUpdatedIndex(activeIndex, selectItems.Count - 1, action));
        StateHasChanged();

        int getUpdatedIndex(int currentIndex, int maxIndex, SelectAction action)
        {
            const int pageSize = 10; // used for pageup/pagedown

            switch (action)
            {
                case SelectAction.First:
                    return 0;
                case SelectAction.Last:
                    return maxIndex;
                case SelectAction.Previous:
                    return Math.Max(0, currentIndex - 1);
                case SelectAction.Next:
                    return Math.Min(maxIndex, currentIndex + 1);
                case SelectAction.PageUp:
                    return Math.Max(0, currentIndex - pageSize);
                case SelectAction.PageDown:
                    return Math.Min(maxIndex, currentIndex + pageSize);
                default:
                    return currentIndex;
            }
        }
    }

    [JSInvokable]
    public async Task selectOptionJS()
    {
        await selectOption(activeIndex);
        StateHasChanged();
    }

    [JSInvokable]
    public async Task updateMenuStateJS(bool toOpen)
    {
        await updateMenuState(toOpen);
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        id = HtmlHelpers.GetRandomHtmlId();
        comboInputId = $"{id}-combo";
        comboMenuId = $"{id}-listbox";
        objRef = DotNetObjectReference.Create(this);

        if (InputAttributes is not null && InputAttributes.TryGetValue("class", out var o))
        {
            if (o is not null && o is string)
                classNameAddition = (string)o;
            else
                Logger.LogWarning("'class' parameter is invalid.");
        }

        Logger.LogInformation("classNameAddition {a}", classNameAddition);
        // labelClassName = LabelHidden ? $"combo-label hidden" : $"combo-label";
        // comboClassName = open ? $"combo open" : $"combo";

        // if (InputAttributes is not null &&
        // {
        //     labelClassName += " " + classNameAddition;
        //     comboClassName += " " + classNameAddition;
        // }
        //var classNameAddition = InputAttributes?.TryGetValue .Where(ia => ia.Key == "class").Select(k => k.Value);

        // className = classNameAddition is not null && classNameAddition.Any()
        //     ? string.Join(" ", "dropdown-button-actions", classNameAddition.First())
    }

    protected override void OnParametersSet()
    {
        if (!SelectedValueChanged.HasDelegate)
        {
            throw new InvalidOperationException($"You must bind the '{nameof(selectedIndex)}' parameter.");
        }

        if (Items.Count() == 0)
        {
            throw new InvalidOperationException("No items specified.");
        }

        selectItems.Clear();
        for (int index = 0; index < Items.Count(); index++)
        {
            selectItems.Add(index, new SelectItem(Items[index], itemId(index)));
        }

        // check initial SelectedValue value exists, if not default to 0
        KeyValuePair<int, SelectItem>? foundEntry =
            selectItems.FirstOrDefault(entry => EqualityComparer<TValue>.Default.Equals(entry.Value.Value, SelectedValue));
        selectedIndex = foundEntry.HasValue ? foundEntry.Value.Key : 0;
        activeIndex = selectedIndex;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Utopia.Blazor.Component/SelectOnlyCombobox.razor.js");
            _jsEventDisposable = await _jsModule.InvokeAsync<IJSObjectReference>("init", comboInputEl, objRef);

            // if (!await JS.InvokeAsync<bool>("selectOnlyCombobox.installOnComboKeyDown", comboInputId, objRef))
            //     Logger.LogError("Failed to install keydown handler.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        timer?.Dispose();
        objRef?.Dispose();

        try
        {
            if (_jsEventDisposable is not null)
            {
                await _jsEventDisposable.InvokeVoidAsync("stop");
                await _jsEventDisposable.DisposeAsync();
            }

            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // The JS side may routinely be gone already if the reason we're disposing is that
            // the client disconnected. This is not an error.
        }
    }

    private class SelectItem
    {
        public TValue? Value { get; set; }
        public string Name { get; private set; }
        public string Id { get; private set; }

        public SelectItem(SelectOption<TValue> option, string id) => (Value, Name, Id) = (option.Value, option.Name, id);
    }
}
