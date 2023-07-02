namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var c = new MyClass<int?>();
            Console.WriteLine(c.TypeName);
        }
    }

    public class MyClass<T>
    {
        public T? Test { get; set; } = default;
        public string TypeName => $"{typeof(T).Name}";
    }
}

