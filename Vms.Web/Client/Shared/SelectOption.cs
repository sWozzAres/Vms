namespace Vms.Web.Client.Shared;

//public class SelectOption(int value, string name)
//{
//    public int Value { get; set; } = value;
//    public string Name { get; set; } = name;
//}

public record SelectOption(int? Value, string Name);
