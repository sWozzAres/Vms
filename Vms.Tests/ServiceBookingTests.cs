using VmsTesting;

namespace Vms.Tests;

public class ServiceBookingTests : IClassFixture<TestDatabaseFixture>
{
    public ServiceBookingTests(TestDatabaseFixture fixture)
    => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public void Test1()
    {
        using var context = TestDatabaseFixture.CreateContext();

        var vehicles = context.Vehicles.ToList();

        Assert.NotEmpty(vehicles);
    }
}