using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vms.Application.Commands.VehicleUseCase;
using VmsTesting;

namespace Vms.Tests;

public class ServiceBookingTests : IClassFixture<TestDatabaseFixture>
{
    readonly TestDatabaseFixture Fixture;

    public ServiceBookingTests(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public async Task Test1()
    {
        var context = New<VmsDbContext>();

        var vehicles = await context.Vehicles.ToListAsync();

        Assert.NotEmpty(vehicles);
    }

    [Fact]
    public void Get_Companies()
    {
        var context = New<VmsDbContext>();

        var companies = context.Companies.ToList();

        Assert.NotEmpty(companies);
    }
    //[Fact]
    //public async Task Assign_ServiceBooking()
    //{
    //    using var context = TestDatabaseFixture.CreateContext();

    //    var vehicle = context.Vehicles.First();

    //    CreateBookingRequest request = new(vehicle.Id, new DateOnly(2023, 1, 1), null, null, false, true);
    //    var serviceBooking = await new CreateServiceBooking(context)
    //        .CreateAsync(request);

    //    //AssignSupplierRequest aRequest = new(serviceBooking.Id);
    //    //var assign = await new AssignSupplier(context, new SupplierLocator(context)).Assign(aRequest);


    //    Assert.Equal("SUP002", serviceBooking.SupplierCode);

    //    //await context.SaveChangesAsync();
    //}

    [Fact]
    public async Task Change_Vrm()
    {
        var context = New<VmsDbContext>();
        var changeVrm = New<ChangeVrm>();

        var vehicle = context.Vehicles.First();

        var request1 = new ChangeVrmRequest(vehicle.Id, "123");
        await changeVrm.ChangeTo(request1);

        var request2 = new ChangeVrmRequest(vehicle.Id, "12345");
        await changeVrm.ChangeTo(request2);

        Assert.Equal("12345", vehicle.Vrm);
    }

    T New<T>() where T : notnull
        => Fixture.ServiceProvider.GetRequiredService<T>();
}