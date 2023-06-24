using System.Buffers.Text;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s1 = "Hello";
            var s2 = s1;

        }
    }

    public record Person
    {
        public int MyInt { get; set; } = 0;
        public string Name { get; set; } = "Fred";
        public Address Address { get; set; } = new();
    }

    public record Address
    {
        public string Street { get; set; } = "MyStreet";
    }
}