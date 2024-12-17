﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using program.Repository.Data;

#nullable disable

namespace program.Repository.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("program.Domain.EnumClasses.Measure", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.HasKey("Id");

                    b.ToTable("Measures");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Kg"
                        },
                        new
                        {
                            Id = 1,
                            Name = "L"
                        },
                        new
                        {
                            Id = 2,
                            Name = "M"
                        },
                        new
                        {
                            Id = 3,
                            Name = "No"
                        });
                });

            modelBuilder.Entity("program.Domain.EnumClasses.ProductStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.ToTable("ProductStatuses");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Updated"
                        },
                        new
                        {
                            Id = 1,
                            Name = "NeedRemoval"
                        });
                });

            modelBuilder.Entity("program.Domain.EnumClasses.Shop", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.ToTable("Shops");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Silpo"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Fozzy"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Fora"
                        });
                });

            modelBuilder.Entity("program.Domain.EnumClasses.Sort", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.HasKey("Id");

                    b.ToTable("Sort");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Name"
                        },
                        new
                        {
                            Id = 1,
                            Name = "UnifiedPrice"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Price"
                        });
                });

            modelBuilder.Entity("program.Domain.EnumClasses.SortOrder", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.HasKey("Id");

                    b.ToTable("SortOrder");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Asc"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Desc"
                        });
                });

            modelBuilder.Entity("program.Domain.Product", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<string>("FullLinkImage")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<string>("FullLinkProduct")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int>("MeasureId")
                        .HasColumnType("integer");

                    b.Property<decimal>("PriceInitial")
                        .HasPrecision(14, 2)
                        .HasColumnType("numeric(14,2)");

                    b.Property<decimal>("PriceUnified")
                        .HasPrecision(14, 2)
                        .HasColumnType("numeric(14,2)");

                    b.Property<int>("ProductStatusId")
                        .HasColumnType("integer");

                    b.HasKey("Name", "ShopId");

                    b.HasIndex("MeasureId");

                    b.HasIndex("ProductStatusId");

                    b.HasIndex("ShopId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("program.Domain.Request", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("SortId")
                        .HasColumnType("integer");

                    b.Property<int>("SortOrderId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "Name");

                    b.HasIndex("SortId");

                    b.HasIndex("SortOrderId");

                    b.ToTable("Requests");

                    b.HasData(
                        new
                        {
                            UserId = -1,
                            Name = "м'ясо",
                            SortId = 0,
                            SortOrderId = 0
                        },
                        new
                        {
                            UserId = -1,
                            Name = "овочі",
                            SortId = 0,
                            SortOrderId = 0
                        },
                        new
                        {
                            UserId = -1,
                            Name = "хліб",
                            SortId = 0,
                            SortOrderId = 0
                        });
                });

            modelBuilder.Entity("program.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Email = "admin@gmail.com",
                            Name = "admin",
                            PasswordHash = "admin"
                        });
                });

            modelBuilder.Entity("program.Domain.Product", b =>
                {
                    b.HasOne("program.Domain.EnumClasses.Measure", "Measure")
                        .WithMany("Products")
                        .HasForeignKey("MeasureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("program.Domain.EnumClasses.ProductStatus", "ProductStatus")
                        .WithMany("Products")
                        .HasForeignKey("ProductStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("program.Domain.EnumClasses.Shop", "Shop")
                        .WithMany("Products")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Measure");

                    b.Navigation("ProductStatus");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("program.Domain.Request", b =>
                {
                    b.HasOne("program.Domain.EnumClasses.Sort", "Sort")
                        .WithMany()
                        .HasForeignKey("SortId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("program.Domain.EnumClasses.SortOrder", "SortOrder")
                        .WithMany()
                        .HasForeignKey("SortOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("program.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sort");

                    b.Navigation("SortOrder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("program.Domain.EnumClasses.Measure", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("program.Domain.EnumClasses.ProductStatus", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("program.Domain.EnumClasses.Shop", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
