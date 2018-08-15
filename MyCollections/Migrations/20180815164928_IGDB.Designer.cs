﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyCollections.Models;

namespace MyCollections.Migrations
{
    [DbContext(typeof(MyCollectionsContext))]
    [Migration("20180815164928_IGDB")]
    partial class IGDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MyCollections.Models.Book", b =>
                {
                    b.Property<int>("BookID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<string>("Author");

                    b.Property<DateTime>("BuyDate");

                    b.Property<bool>("EBook");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<float>("Price");

                    b.HasKey("BookID");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("MyCollections.Models.Game", b =>
                {
                    b.Property<int>("GameID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<DateTime?>("BuyDate");

                    b.Property<string>("Cover");

                    b.Property<string>("FriendlyName");

                    b.Property<int?>("IGDBId");

                    b.Property<string>("Logo");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("PlayedTime");

                    b.Property<float?>("Price");

                    b.Property<bool>("Purchased");

                    b.Property<int?>("SteamApID");

                    b.Property<int?>("StoreID");

                    b.Property<int>("SystemID");

                    b.HasKey("GameID");

                    b.HasIndex("StoreID");

                    b.HasIndex("SystemID");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("MyCollections.Models.Param", b =>
                {
                    b.Property<string>("key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("value");

                    b.HasKey("key");

                    b.ToTable("Param");
                });

            modelBuilder.Entity("MyCollections.Models.Store", b =>
                {
                    b.Property<int>("StoreID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Logo");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("StoreID");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("MyCollections.Models.System", b =>
                {
                    b.Property<int>("SystemID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Logo");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("SystemID");

                    b.ToTable("System");
                });

            modelBuilder.Entity("MyCollections.Models.Game", b =>
                {
                    b.HasOne("MyCollections.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreID");

                    b.HasOne("MyCollections.Models.System", "System")
                        .WithMany()
                        .HasForeignKey("SystemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
