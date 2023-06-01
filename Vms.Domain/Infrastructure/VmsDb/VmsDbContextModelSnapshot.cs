﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Vms.Domain.Infrastructure;

#nullable disable

namespace Vms.Domain.Infrastructure.VmsDb
{
    [DbContext(typeof(VmsDbContext))]
    partial class VmsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NetworkSupplier", b =>
                {
                    b.Property<int>("NetworkId")
                        .HasColumnType("int");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("NetworkId", "SupplierId");

                    b.HasIndex("SupplierId");

                    b.ToTable("NetworkSupplier", (string)null);
                });

            modelBuilder.Entity("SupplierFranchise", b =>
                {
                    b.Property<string>("SupplierCode")
                        .HasMaxLength(8)
                        .IsUnicode(false)
                        .HasColumnType("varchar(8)");

                    b.Property<string>("Franchise")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.HasKey("SupplierCode", "Franchise");

                    b.HasIndex("Franchise");

                    b.ToTable("SupplierFranchise", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("CompanyCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Code" }, "IX_Company")
                        .IsUnique();

                    b.ToTable("Company", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("CompanyCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "CompanyCode", "Code" }, "IX_Customer")
                        .IsUnique();

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.CustomerNetwork", b =>
                {
                    b.Property<string>("CompanyCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("NetworkCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("CustomerCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.HasKey("CompanyCode", "NetworkCode", "CustomerCode");

                    b.HasIndex("CompanyCode", "CustomerCode");

                    b.ToTable("CustomerNetwork", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.Driver", b =>
                {
                    b.Property<string>("EmailAddress")
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<Geometry>("HomeLocation")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("MiddleNames")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasMaxLength(12)
                        .IsUnicode(false)
                        .HasColumnType("varchar(12)");

                    b.Property<string>("Salutation")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)");

                    b.HasKey("EmailAddress")
                        .HasName("PK_Driver_1");

                    b.ToTable("Driver", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.DriverVehicle", b =>
                {
                    b.Property<string>("EmailAddress")
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("EmailAddress");

                    b.HasIndex("VehicleId");

                    b.ToTable("DriverVehicles");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Fleet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("CompanyCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "CompanyCode", "Code" }, "IX_Fleet")
                        .IsUnique();

                    b.ToTable("Fleet", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.FleetNetwork", b =>
                {
                    b.Property<string>("CompanyCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("FleetCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("NetworkCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.HasKey("CompanyCode", "FleetCode", "NetworkCode");

                    b.HasIndex("CompanyCode", "NetworkCode");

                    b.ToTable("FleetNetwork", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.Network", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("CompanyCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "CompanyCode", "Code" }, "IX_Network")
                        .IsUnique();

                    b.ToTable("Network", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.ServiceBooking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("MotDue")
                        .HasColumnType("date");

                    b.Property<DateOnly>("PreferredDate1")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("PreferredDate2")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("PreferredDate3")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("ServiceBooking", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(8)
                        .IsUnicode(false)
                        .HasColumnType("varchar(8)");

                    b.Property<bool>("IsIndependent")
                        .HasColumnType("bit");

                    b.Property<Geometry>("Location")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasMaxLength(9)
                        .IsUnicode(false)
                        .HasColumnType("varchar(9)");

                    b.HasKey("Id");

                    b.ToTable("Supplier", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ChassisNumber")
                        .HasMaxLength(18)
                        .IsUnicode(false)
                        .HasColumnType("varchar(18)");

                    b.Property<string>("CompanyCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("CustomerCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<DateOnly>("DateFirstRegistered")
                        .HasColumnType("date");

                    b.Property<string>("FleetCode")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyCode", "CustomerCode");

                    b.HasIndex("CompanyCode", "FleetCode");

                    b.HasIndex("Make", "Model");

                    b.ToTable("Vehicle", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.VehicleMake", b =>
                {
                    b.Property<string>("Make")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.HasKey("Make")
                        .HasName("PK_Make");

                    b.ToTable("VehicleMake", (string)null);
                });

            modelBuilder.Entity("Vms.Domain.Entity.VehicleModel", b =>
                {
                    b.Property<string>("Make")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Model")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Make", "Model")
                        .HasName("PK_Model");

                    b.ToTable("VehicleModel", (string)null);
                });

            modelBuilder.Entity("NetworkSupplier", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Network", null)
                        .WithMany()
                        .HasForeignKey("NetworkId")
                        .IsRequired()
                        .HasConstraintName("FK_NetworkSupplier_Network");

                    b.HasOne("Vms.Domain.Entity.Supplier", null)
                        .WithMany()
                        .HasForeignKey("SupplierId")
                        .IsRequired()
                        .HasConstraintName("FK_NetworkSupplier_Supplier");
                });

            modelBuilder.Entity("SupplierFranchise", b =>
                {
                    b.HasOne("Vms.Domain.Entity.VehicleMake", null)
                        .WithMany()
                        .HasForeignKey("Franchise")
                        .IsRequired()
                        .HasConstraintName("FK_SupplierFranchise_VehicleMake");

                    b.HasOne("Vms.Domain.Entity.Supplier", null)
                        .WithMany()
                        .HasForeignKey("SupplierCode")
                        .HasPrincipalKey("Code")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_SupplierFranchise_Supplier");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Customer", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Company", "CompanyCodeNavigation")
                        .WithMany("Customers")
                        .HasForeignKey("CompanyCode")
                        .HasPrincipalKey("Code")
                        .IsRequired()
                        .HasConstraintName("FK_Customer_Company");

                    b.Navigation("CompanyCodeNavigation");
                });

            modelBuilder.Entity("Vms.Domain.Entity.CustomerNetwork", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Customer", "C")
                        .WithMany("CustomerNetworks")
                        .HasForeignKey("CompanyCode", "CustomerCode")
                        .HasPrincipalKey("CompanyCode", "Code")
                        .IsRequired()
                        .HasConstraintName("FK_CustomerNetwork_Customer");

                    b.HasOne("Vms.Domain.Entity.Network", "Network")
                        .WithMany("CustomerNetworks")
                        .HasForeignKey("CompanyCode", "NetworkCode")
                        .HasPrincipalKey("CompanyCode", "Code")
                        .IsRequired()
                        .HasConstraintName("FK_CustomerNetwork_Network");

                    b.Navigation("C");

                    b.Navigation("Network");
                });

            modelBuilder.Entity("Vms.Domain.Entity.DriverVehicle", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Driver", "EmailAddressNavigation")
                        .WithOne("DriverVehicle")
                        .HasForeignKey("Vms.Domain.Entity.DriverVehicle", "EmailAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DriverVehicles_Driver");

                    b.HasOne("Vms.Domain.Entity.Vehicle", "Vehicle")
                        .WithMany("DriverVehicles")
                        .HasForeignKey("VehicleId")
                        .IsRequired()
                        .HasConstraintName("FK_DriverVehicles_Vehicle");

                    b.Navigation("EmailAddressNavigation");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Fleet", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Company", "CompanyCodeNavigation")
                        .WithMany("Fleets")
                        .HasForeignKey("CompanyCode")
                        .HasPrincipalKey("Code")
                        .IsRequired()
                        .HasConstraintName("FK_Fleet_Company");

                    b.Navigation("CompanyCodeNavigation");
                });

            modelBuilder.Entity("Vms.Domain.Entity.FleetNetwork", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Fleet", "Fleet")
                        .WithMany("FleetNetworks")
                        .HasForeignKey("CompanyCode", "FleetCode")
                        .HasPrincipalKey("CompanyCode", "Code")
                        .IsRequired()
                        .HasConstraintName("FK_FleetNetwork_Fleet");

                    b.HasOne("Vms.Domain.Entity.Network", "Network")
                        .WithMany("FleetNetworks")
                        .HasForeignKey("CompanyCode", "NetworkCode")
                        .HasPrincipalKey("CompanyCode", "Code")
                        .IsRequired()
                        .HasConstraintName("FK_FleetNetwork_Network");

                    b.Navigation("Fleet");

                    b.Navigation("Network");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Network", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Company", "CompanyCodeNavigation")
                        .WithMany("Networks")
                        .HasForeignKey("CompanyCode")
                        .HasPrincipalKey("Code")
                        .IsRequired()
                        .HasConstraintName("FK_Network_Company");

                    b.Navigation("CompanyCodeNavigation");
                });

            modelBuilder.Entity("Vms.Domain.Entity.ServiceBooking", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Vehicle", "Vehicle")
                        .WithMany("ServiceBookings")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Vms.Domain.Entity.ServiceBookingSupplier", "Supplier", b1 =>
                        {
                            b1.Property<int>("ServiceBookingId")
                                .HasColumnType("int");

                            b1.Property<int>("SupplierId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("ValidFrom")
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("datetime2")
                                .HasColumnName("ValidFrom");

                            b1.Property<DateTime>("ValidTo")
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("datetime2")
                                .HasColumnName("ValidTo");

                            b1.HasKey("ServiceBookingId");

                            b1.HasIndex("SupplierId");

                            b1.ToTable("ServiceBookingSupplier", (string)null);

                            b1.ToTable(tb => tb.IsTemporal(ttb =>
                                    {
                                        ttb.UseHistoryTable("ServiceBookingSupplierHistory");
                                        ttb
                                            .HasPeriodStart("ValidFrom")
                                            .HasColumnName("ValidFrom");
                                        ttb
                                            .HasPeriodEnd("ValidTo")
                                            .HasColumnName("ValidTo");
                                    }));

                            b1.WithOwner("ServiceBooking")
                                .HasForeignKey("ServiceBookingId");

                            b1.HasOne("Vms.Domain.Entity.Supplier", "Supplier")
                                .WithMany()
                                .HasForeignKey("SupplierId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.Navigation("ServiceBooking");

                            b1.Navigation("Supplier");
                        });

                    b.Navigation("Supplier");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Vehicle", b =>
                {
                    b.HasOne("Vms.Domain.Entity.Company", "CompanyCodeNavigation")
                        .WithMany("Vehicles")
                        .HasForeignKey("CompanyCode")
                        .HasPrincipalKey("Code")
                        .IsRequired()
                        .HasConstraintName("FK_Vehicle_Company");

                    b.HasOne("Vms.Domain.Entity.Customer", "C")
                        .WithMany("Vehicles")
                        .HasForeignKey("CompanyCode", "CustomerCode")
                        .HasPrincipalKey("CompanyCode", "Code")
                        .HasConstraintName("FK_Vehicle_Customer");

                    b.HasOne("Vms.Domain.Entity.Fleet", "Fleet")
                        .WithMany("Vehicles")
                        .HasForeignKey("CompanyCode", "FleetCode")
                        .HasPrincipalKey("CompanyCode", "Code")
                        .HasConstraintName("FK_Vehicle_Fleet");

                    b.HasOne("Vms.Domain.Entity.VehicleModel", "M")
                        .WithMany("Vehicles")
                        .HasForeignKey("Make", "Model")
                        .IsRequired()
                        .HasConstraintName("FK_Vehicle_VehicleModel");

                    b.OwnsOne("Vms.Domain.Entity.VehicleMot", "Mot", b1 =>
                        {
                            b1.Property<int>("VehicleId")
                                .HasColumnType("int");

                            b1.Property<DateOnly>("Due")
                                .HasColumnType("date");

                            b1.HasKey("VehicleId");

                            b1.ToTable("VehicleMot", (string)null);

                            b1.WithOwner("Vehicle")
                                .HasForeignKey("VehicleId")
                                .HasConstraintName("FK_Vehicle_VehicleMot");

                            b1.Navigation("Vehicle");
                        });

                    b.OwnsOne("Vms.Domain.Entity.VehicleVrm", "Vrm", b1 =>
                        {
                            b1.Property<int>("VehicleId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("ValidFrom")
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("datetime2")
                                .HasColumnName("ValidFrom");

                            b1.Property<DateTime>("ValidTo")
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("datetime2")
                                .HasColumnName("ValidTo");

                            b1.Property<string>("Vrm")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("VehicleId");

                            b1.ToTable("VehicleVrm", (string)null);

                            b1.ToTable(tb => tb.IsTemporal(ttb =>
                                    {
                                        ttb.UseHistoryTable("VehicleVrmHistory");
                                        ttb
                                            .HasPeriodStart("ValidFrom")
                                            .HasColumnName("ValidFrom");
                                        ttb
                                            .HasPeriodEnd("ValidTo")
                                            .HasColumnName("ValidTo");
                                    }));

                            b1.WithOwner("Vehicle")
                                .HasForeignKey("VehicleId")
                                .HasConstraintName("FK_Vehicle_VehicleVrm");

                            b1.Navigation("Vehicle");
                        });

                    b.Navigation("C");

                    b.Navigation("CompanyCodeNavigation");

                    b.Navigation("Fleet");

                    b.Navigation("M");

                    b.Navigation("Mot")
                        .IsRequired();

                    b.Navigation("Vrm")
                        .IsRequired();
                });

            modelBuilder.Entity("Vms.Domain.Entity.VehicleModel", b =>
                {
                    b.HasOne("Vms.Domain.Entity.VehicleMake", "MakeNavigation")
                        .WithMany("VehicleModels")
                        .HasForeignKey("Make")
                        .IsRequired();

                    b.Navigation("MakeNavigation");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Company", b =>
                {
                    b.Navigation("Customers");

                    b.Navigation("Fleets");

                    b.Navigation("Networks");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Customer", b =>
                {
                    b.Navigation("CustomerNetworks");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Driver", b =>
                {
                    b.Navigation("DriverVehicle")
                        .IsRequired();
                });

            modelBuilder.Entity("Vms.Domain.Entity.Fleet", b =>
                {
                    b.Navigation("FleetNetworks");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Network", b =>
                {
                    b.Navigation("CustomerNetworks");

                    b.Navigation("FleetNetworks");
                });

            modelBuilder.Entity("Vms.Domain.Entity.Vehicle", b =>
                {
                    b.Navigation("DriverVehicles");

                    b.Navigation("ServiceBookings");
                });

            modelBuilder.Entity("Vms.Domain.Entity.VehicleMake", b =>
                {
                    b.Navigation("VehicleModels");
                });

            modelBuilder.Entity("Vms.Domain.Entity.VehicleModel", b =>
                {
                    b.Navigation("Vehicles");
                });
#pragma warning restore 612, 618
        }
    }
}
