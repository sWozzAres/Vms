using ConsoleApp1.Db;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            var factory = new TestContextFactory();
            using var context = factory.CreateDbContext(Array.Empty<string>());

            var serviceBooking = new ServiceBooking();
            var vehicle = new Vehicle();
            var motEvent = new MotEvent(serviceBooking.Id, vehicle.Id);

            context.MotEvents.Add(motEvent);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}