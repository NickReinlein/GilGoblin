﻿// <auto-generated />
using System;
using GilGoblin.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GilGoblin.Migrations
{
    [DbContext(typeof(ItemDBContext))]
    [Migration("20220212210933_ItemInfoDB")]
    partial class ItemInfoDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.22");

            modelBuilder.Entity("GilGoblin.Database.ItemDB", b =>
                {
                    b.Property<int>("itemID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("itemInfoitemID")
                        .HasColumnType("INTEGER");

                    b.HasKey("itemID");

                    b.HasIndex("itemInfoitemID");

                    b.ToTable("ItemDB");
                });

            modelBuilder.Entity("GilGoblin.Database.ItemInfoDB", b =>
                {
                    b.Property<int>("itemID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("description")
                        .HasColumnType("TEXT");

                    b.Property<int>("gatheringID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("iconID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .HasColumnType("TEXT");

                    b.Property<int>("stack_size")
                        .HasColumnType("INTEGER");

                    b.Property<int>("vendor_price")
                        .HasColumnType("INTEGER");

                    b.HasKey("itemID");

                    b.ToTable("ItemInfoDB");
                });

            modelBuilder.Entity("GilGoblin.Database.MarketDataDB", b =>
                {
                    b.Property<int>("itemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("worldID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("averagePrice")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("lastUpdated")
                        .HasColumnType("TEXT");

                    b.HasKey("itemID", "worldID");

                    b.ToTable("MarketData");
                });

            modelBuilder.Entity("GilGoblin.Database.MarketListingDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MarketDataDBitemID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MarketDataDBworldID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("hq")
                        .HasColumnType("INTEGER");

                    b.Property<int>("item_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("price")
                        .HasColumnType("INTEGER");

                    b.Property<int>("qty")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("timestamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("world_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MarketDataDBitemID", "MarketDataDBworldID");

                    b.ToTable("MarketListing");
                });

            modelBuilder.Entity("GilGoblin.Database.RecipeDB", b =>
                {
                    b.Property<int>("recipe_id")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanHq")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanQuickSynth")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ItemDBitemID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ItemInfoDBitemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("icon_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("result_quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("target_item_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("recipe_id");

                    b.HasIndex("ItemDBitemID");

                    b.HasIndex("ItemInfoDBitemID");

                    b.ToTable("RecipeDB");
                });

            modelBuilder.Entity("GilGoblin.Finance.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("RecipeDBrecipe_id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("RecipeFullWebrecipe_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("item_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RecipeDBrecipe_id");

                    b.HasIndex("RecipeFullWebrecipe_id");

                    b.ToTable("Ingredient");
                });

            modelBuilder.Entity("GilGoblin.WebAPI.ItemInfoWeb", b =>
                {
                    b.Property<int>("itemID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("description")
                        .HasColumnType("TEXT");

                    b.Property<int>("gatheringID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("iconID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .HasColumnType("TEXT");

                    b.Property<int>("stack_size")
                        .HasColumnType("INTEGER");

                    b.Property<int>("vendor_price")
                        .HasColumnType("INTEGER");

                    b.HasKey("itemID");

                    b.ToTable("ItemInfoWeb");
                });

            modelBuilder.Entity("GilGoblin.WebAPI.ItemRecipeHeaderAPI", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ItemInfoWebitemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("class_job_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("level")
                        .HasColumnType("INTEGER");

                    b.Property<int>("recipe_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("ItemInfoWebitemID");

                    b.ToTable("ItemRecipeHeaderAPI");
                });

            modelBuilder.Entity("GilGoblin.WebAPI.RecipeFullWeb", b =>
                {
                    b.Property<int>("recipe_id")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanHq")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanQuickSynth")
                        .HasColumnType("INTEGER");

                    b.Property<int>("icon_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("result_quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("target_item_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("recipe_id");

                    b.ToTable("RecipeFullWeb");
                });

            modelBuilder.Entity("GilGoblin.Database.ItemDB", b =>
                {
                    b.HasOne("GilGoblin.Database.ItemInfoDB", "itemInfo")
                        .WithMany()
                        .HasForeignKey("itemInfoitemID");
                });

            modelBuilder.Entity("GilGoblin.Database.MarketDataDB", b =>
                {
                    b.HasOne("GilGoblin.Database.ItemDB", null)
                        .WithMany("marketData")
                        .HasForeignKey("itemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GilGoblin.Database.MarketListingDB", b =>
                {
                    b.HasOne("GilGoblin.Database.MarketDataDB", null)
                        .WithMany("listings")
                        .HasForeignKey("MarketDataDBitemID", "MarketDataDBworldID");
                });

            modelBuilder.Entity("GilGoblin.Database.RecipeDB", b =>
                {
                    b.HasOne("GilGoblin.Database.ItemDB", null)
                        .WithMany("fullRecipes")
                        .HasForeignKey("ItemDBitemID");

                    b.HasOne("GilGoblin.Database.ItemInfoDB", null)
                        .WithMany("fullRecipes")
                        .HasForeignKey("ItemInfoDBitemID");
                });

            modelBuilder.Entity("GilGoblin.Finance.Ingredient", b =>
                {
                    b.HasOne("GilGoblin.Database.RecipeDB", null)
                        .WithMany("ingredients")
                        .HasForeignKey("RecipeDBrecipe_id");

                    b.HasOne("GilGoblin.WebAPI.RecipeFullWeb", null)
                        .WithMany("ingredients")
                        .HasForeignKey("RecipeFullWebrecipe_id");
                });

            modelBuilder.Entity("GilGoblin.WebAPI.ItemRecipeHeaderAPI", b =>
                {
                    b.HasOne("GilGoblin.WebAPI.ItemInfoWeb", null)
                        .WithMany("recipeHeader")
                        .HasForeignKey("ItemInfoWebitemID");
                });
#pragma warning restore 612, 618
        }
    }
}