namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var p1 = new Person("Fred", new Address("Alexandra Close"));
            var p2 = p1 with { Name = "John" };
            p1.Name = "x";

            Console.WriteLine(p1);
            Console.WriteLine(p2);

            return;

            var xi = NT<int>.MyFunc<int>();
            var x = NT<int?>.MyFunc<int?>();
            var xs = NT<string>.MyFunc<string>();

            static T? MyFunc<T>() 
            {
                return default(T);
            }

            var so = SelectOption<int>.GetPrompt<int?>("Hello");
            Console.Write("so[0].Value: ");
            Console.WriteLine(so[0].Value is null ? "null" : so[0].Value);

            //var ss = SelectOption<int?>.GetPrompt<int?>("Hello2");
            //Console.Write("GetPrompt2: ");
            //Console.WriteLine(ss.Value is null ? "null": ss.Value);

            Console.Write("xi: ");
            Console.WriteLine(xi.Value);
            Console.Write("x: ");
            Console.WriteLine(x.Value is null ? "null" : x.Value);
            Console.Write("xs: ");
            Console.WriteLine(xs.Value is null ? "null" : xs.Value);

            var r = new[] { (1, "one"), (2, "two") };

            var options = SelectOption<int>.WithPrompt<int?>("Test", new (int?, string) [] { (1,"one"),  (2,"two") });
            foreach(var option in options)
            {
                Console.WriteLine(option);
            }

            var makes = new List<X>() { new X() { Make = "M1" }, new X() { Make = "M2" } };
            var options2 = SelectOption<string>.WithPrompt("Test", 
                makes.Select(x=>(x.Make,x.Make)));
            foreach (var option in options2)
            {
                Console.WriteLine(option);
            }

            var makesi = new List<XI>() { new XI() { Make = 1 }, new XI() { Make = 2 } };
            var options2i = SelectOption<int?>.WithPrompt("Test",
                makesi.Select(x => new SelectOption<int>(x.Make, x.Make.ToString() )));
            foreach (var option in options2i)
            {
                Console.WriteLine(option);
            }


        }
    }

    public class X
    {
        public string Make { get; set; }
    }
    public class XI
    {
        public int Make { get; set; }
    }

    public record NT<TValue>(TValue? Value)  
    {
        public static NT<T?> MyFunc<T>()
        {
            return new NT<T?>(default(T));
        }
    }
    public record SelectOption<TValue>(TValue? Value, string Name)
    {
        public static SelectOption<T?>[] GetPrompt<T>(string prompt)
            => new SelectOption<T?>[] { new(default(T), $"-- Select {prompt} --") };

        public static SelectOption<T?>[] WithPrompt<T>(string prompt, IEnumerable<SelectOption<T?>> items)
            => GetPrompt<T>(prompt).Concat(items).ToArray();

        public static SelectOption<T?>[] WithPrompt<T>(string prompt, IEnumerable<(T Value, string Name)> items)
            => GetPrompt<T>(prompt).Concat(items.Select(i => new SelectOption<T?>(i.Value, i.Name))).ToArray();
    }

    public record struct Person (string Name, Address Address)
    {
        public void ChangeName(string name) => Name = name;
    };

    public record struct Address(string Street);
    
}

