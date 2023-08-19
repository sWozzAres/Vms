using TestEf.Db;

namespace TestEf
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new TestContextFactory();
            using var context = factory.CreateDbContext(Array.Empty<string>());

            var serviceBooking = new ServiceBooking();
            var vehicle = new Vehicle();
            var motEvent = new MotEvent() {
                ServiceBooking = serviceBooking,
                Vehicle = vehicle,
            };

            context.ServiceBookings.Add(serviceBooking);
            context.Vehicles.Add(vehicle);
            context.MotEvents.Add(motEvent);

            var s1 = context.ServiceBookings.Find(new object[] { serviceBooking.Id });

            var s2 = context.ServiceBookings.SingleOrDefault(x=>x.Id == serviceBooking.Id);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
