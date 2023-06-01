using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using System;
using Vms.Domain.UseCase;
using VmsTesting;

namespace Vms.Tests;

public class ServiceBookingTests : IClassFixture<TestDatabaseFixture>
{
    readonly TestDatabaseFixture Fixture;
    readonly ServiceProvider ServiceProvider;
    readonly ILoggerFactory LoggerFactory;

    public ServiceBookingTests(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
        ServiceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddDebug())
            .BuildServiceProvider();
        LoggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
    }

    [Fact]
    public void Test1()
    {
        using var context = TestDatabaseFixture.CreateContext();

        var vehicles = context.Vehicles.ToList();

        Assert.NotEmpty(vehicles);
    }

    [Fact]
    public async Task Create_ServiceBooking()
    {
        using var context = TestDatabaseFixture.CreateContext();

        var vehicle = context.Vehicles.First();

        CreateBookingRequest request = new(vehicle.Id, new DateOnly(2023, 1, 1), null, null, true);
        var response = await new CreateServiceBooking(context, LoggerFactory.CreateLogger<CreateServiceBooking>()).CreateAsync(request);
        
    }

    [Fact]
    public async Task Change_Vrm()
    {
        using var context = TestDatabaseFixture.CreateContext();

        var vehicle = context.Vehicles.First();

        ChangeVrmRequest request1 = new ChangeVrmRequest(vehicle.Id, "123");
        var response1 = await new ChangeVrm(context).ChangeTo(request1);

        ChangeVrmRequest request2 = new ChangeVrmRequest(vehicle.Id, "12345");
        var response2 = await new ChangeVrm(context).ChangeTo(request2);
    }
}