﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Vms.Data.Models;

public partial class Supplier
{
    public string Code { get; set; } = null!;

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string AddressStreet { get; set; } = null!;

    public string AddressLocality { get; set; } = null!;

    public string AddressTown { get; set; } = null!;

    public string AddressPostcode { get; set; } = null!;

    public Geometry AddressLocation { get; set; } = null!;

    public bool IsIndependent { get; set; }

    public virtual ICollection<ServiceBookingSupplier> ServiceBookingSupplier { get; set; } = new List<ServiceBookingSupplier>();

    public virtual ICollection<VehicleMake> Franchise { get; set; } = new List<VehicleMake>();

    public virtual ICollection<Network> Network { get; set; } = new List<Network>();
}