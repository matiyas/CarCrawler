﻿// <auto-generated />
using System;
using CarCrawler.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

#nullable disable

namespace CarCrawler.Migrations
{
    [DbContext(typeof(CarCrawlerDbContext))]
    partial class CarCrawlerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.16");

            modelBuilder.Entity("CarCrawler.Database.AdDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Brand")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("FuelType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ISOCurrencySymbol")
                        .HasColumnType("TEXT");

                    b.Property<uint?>("MileageKilometers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Model")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Price")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("RegistrationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("TEXT");

                    b.Property<Point>("SellerCoordinates")
                        .HasColumnType("POINT");

                    b.Property<string>("SellerPhones")
                        .HasColumnType("json");

                    b.Property<int?>("TravelDistance")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan?>("TravelDuration")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<string>("VIN")
                        .HasColumnType("TEXT");

                    b.Property<string>("Year")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AdDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
