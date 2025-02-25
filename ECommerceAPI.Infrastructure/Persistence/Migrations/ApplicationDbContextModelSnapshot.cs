﻿// <auto-generated />
using System;
using ECommerceAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ECommerceAPI.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ECommerceAPI.Core.Entities.OrderDetailEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("OrderEntityId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ProductEntityId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("OrderEntityId");

                    b.HasIndex("ProductEntityId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("ECommerceAPI.Core.Entities.OrderEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CustomerEmail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CustomerGSM")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ECommerceAPI.Core.Entities.ProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ECommerceAPI.Core.Entities.OrderDetailEntity", b =>
                {
                    b.HasOne("ECommerceAPI.Core.Entities.OrderEntity", "OrderEntity")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECommerceAPI.Core.Entities.ProductEntity", "ProductEntity")
                        .WithMany()
                        .HasForeignKey("ProductEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderEntity");

                    b.Navigation("ProductEntity");
                });

            modelBuilder.Entity("ECommerceAPI.Core.Entities.OrderEntity", b =>
                {
                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
