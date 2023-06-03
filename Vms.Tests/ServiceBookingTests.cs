using Microsoft.EntityFrameworkCore;
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
    public async Task Test1()
    {
        using var context = TestDatabaseFixture.CreateContext();

        var vehicles = await context.Vehicles.ToListAsync();

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

        var request1 = new ChangeVrmRequest(vehicle.Id, "123");
        await new ChangeVrm(context).ChangeTo(request1);

        var request2 = new ChangeVrmRequest(vehicle.Id, "12345");
        await new ChangeVrm(context).ChangeTo(request2);

        Assert.Equal("12345", vehicle.Vrm);
    }
}