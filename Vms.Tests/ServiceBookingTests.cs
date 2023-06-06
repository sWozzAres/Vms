using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NuGet.Frameworks;
using System;
using Vms.Application;
using Vms.Application.Services;
using Vms.Application.UseCase;
using Vms.Domain.Services;
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
    public async Task Assign_ServiceBooking()
    {
        using var context = TestDatabaseFixture.CreateContext();

        var vehicle = context.Vehicles.First();

        CreateBookingRequest request = new(vehicle.Id, new DateOnly(2023, 1, 1), null, null, true);
        var serviceBooking = await new CreateServiceBooking(context)
            .CreateAsync(request);

        //AssignSupplierRequest aRequest = new(serviceBooking.Id);
        //var assign = await new AssignSupplier(context, new SupplierLocator(context)).Assign(aRequest);


        Assert.Equal("SUP002", serviceBooking.Supplier?.SupplierCode);

        //await context.SaveChangesAsync();
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