﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineShopV1.DAL;

namespace OnlineShopV1.Migrations
{
    [DbContext(typeof(OnlineShopDbContext))]
    [Migration("20190205134400_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity("OnlineShopV1.Admin", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("LastLogin");

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.HasKey("ID");

                    b.ToTable("Admins");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            LastLogin = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Password = "AQAAAAEAACcQAAAAELd1t3QgFbjO+GuhHMjy7VjnKCaR3Fi015Bzwb3qU3cNhXQiJ2j1yrE6PSkg+Y1twQ==",
                            Username = "omidd"
                        });
                });

            modelBuilder.Entity("OnlineShopV1.Authentication", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<DateTime>("Expires");

                    b.Property<int>("UserType");

                    b.Property<string>("Username");

                    b.HasKey("ID");

                    b.ToTable("Authentications");

                    b.HasAnnotation("MySQL:Charset", "utf8mb4");
                });

            modelBuilder.Entity("OnlineShopV1.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
