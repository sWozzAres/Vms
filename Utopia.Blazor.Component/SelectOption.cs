namespace Utopia.Blazor.Component;

//public record SelectOption<TValue>(TValue? Value, string Name)
//{
//    private static SelectOption<T>[] GetPrompt<T>(string prompt)
//        => new SelectOption<T>[] { new(default, $"-- Select {prompt} --") };

//    public static SelectOption<T>[] WithPrompt<T>(string prompt, IEnumerable<SelectOption<T>> items)
//        => GetPrompt<T>(prompt).Concat(items).ToArray();

//    public static SelectOption<T>[] WithPrompt<T>(string prompt, IEnumerable<(T Value, string Name)> items)
//        => GetPrompt<T>(prompt).Concat(items.Select(i => new SelectOption<T>(i.Value, i.Name))).ToArray();
//}


public record SelectOption<TValue>(TValue? Value, string Name)
{
    public static SelectOption<T?>[] GetPrompt<T>(string prompt)
        => new SelectOption<T?>[] { new(default(T), $"-- Select {prompt} --") };

    public static SelectOption<T?>[] WithPrompt<T>(string prompt, IEnumerable<SelectOption<T?>> items)
        => GetPrompt<T>(prompt).Concat(items).ToArray();

    public static SelectOption<T?>[] WithPrompt<T>(string prompt, IEnumerable<(T Value, string Name)> items)
        => GetPrompt<T>(prompt).Concat(items.Select(i => new SelectOption<T?>(i.Value, i.Name))).ToArray();
}