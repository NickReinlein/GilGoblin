using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GilGoblin.Migrations
{
    public partial class abct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemInfoDB",
                columns: table => new
                {
                    itemID = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    iconID = table.Column<int>(nullable: false),
                    vendor_price = table.Column<int>(nullable: false),
                    stack_size = table.Column<int>(nullable: false),
                    gatheringID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemInfoDB", x => x.itemID);
                });

            migrationBuilder.CreateTable(
                name: "ItemInfoWeb",
                columns: table => new
                {
                    itemID = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    iconID = table.Column<int>(nullable: false),
                    vendor_price = table.Column<int>(nullable: false),
                    stack_size = table.Column<int>(nullable: false),
                    gatheringID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemInfoWeb", x => x.itemID);
                });

            migrationBuilder.CreateTable(
                name: "RecipeFullWeb",
                columns: table => new
                {
                    recipe_id = table.Column<int>(nullable: false),
                    target_item_id = table.Column<int>(nullable: false),
                    icon_id = table.Column<int>(nullable: false),
                    result_quantity = table.Column<int>(nullable: false),
                    CanHq = table.Column<bool>(nullable: false),
                    CanQuickSynth = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeFullWeb", x => x.recipe_id);
                });

            migrationBuilder.CreateTable(
                name: "ItemDB",
                columns: table => new
                {
                    itemID = table.Column<int>(nullable: false),
                    itemInfoitemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDB", x => x.itemID);
                    table.ForeignKey(
                        name: "FK_ItemDB_ItemInfoDB_itemInfoitemID",
                        column: x => x.itemInfoitemID,
                        principalTable: "ItemInfoDB",
                        principalColumn: "itemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemRecipeHeaderAPI",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    class_job_id = table.Column<int>(nullable: false),
                    recipe_id = table.Column<int>(nullable: false),
                    level = table.Column<int>(nullable: false),
                    ItemInfoWebitemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRecipeHeaderAPI", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemRecipeHeaderAPI_ItemInfoWeb_ItemInfoWebitemID",
                        column: x => x.ItemInfoWebitemID,
                        principalTable: "ItemInfoWeb",
                        principalColumn: "itemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketData",
                columns: table => new
                {
                    worldID = table.Column<int>(nullable: false),
                    itemID = table.Column<int>(nullable: false),
                    lastUpdated = table.Column<DateTime>(nullable: false),
                    averagePrice = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketData", x => new { x.itemID, x.worldID });
                    table.ForeignKey(
                        name: "FK_MarketData_ItemDB_itemID",
                        column: x => x.itemID,
                        principalTable: "ItemDB",
                        principalColumn: "itemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeDB",
                columns: table => new
                {
                    recipe_id = table.Column<int>(nullable: false),
                    target_item_id = table.Column<int>(nullable: false),
                    icon_id = table.Column<int>(nullable: false),
                    result_quantity = table.Column<int>(nullable: false),
                    CanHq = table.Column<bool>(nullable: false),
                    CanQuickSynth = table.Column<bool>(nullable: false),
                    ItemDBitemID = table.Column<int>(nullable: true),
                    ItemInfoDBitemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeDB", x => x.recipe_id);
                    table.ForeignKey(
                        name: "FK_RecipeDB_ItemDB_ItemDBitemID",
                        column: x => x.ItemDBitemID,
                        principalTable: "ItemDB",
                        principalColumn: "itemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeDB_ItemInfoDB_ItemInfoDBitemID",
                        column: x => x.ItemInfoDBitemID,
                        principalTable: "ItemInfoDB",
                        principalColumn: "itemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketListing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    hq = table.Column<bool>(nullable: false),
                    price = table.Column<int>(nullable: false),
                    qty = table.Column<int>(nullable: false),
                    item_id = table.Column<int>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false),
                    MarketDataDBitemID = table.Column<int>(nullable: true),
                    MarketDataDBworldID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketListing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketListing_MarketData_MarketDataDBitemID_MarketDataDBworldID",
                        columns: x => new { x.MarketDataDBitemID, x.MarketDataDBworldID },
                        principalTable: "MarketData",
                        principalColumns: new[] { "itemID", "worldID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    item_id = table.Column<int>(nullable: false),
                    quantity = table.Column<int>(nullable: false),
                    RecipeDBrecipe_id = table.Column<int>(nullable: true),
                    RecipeFullWebrecipe_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredient_RecipeDB_RecipeDBrecipe_id",
                        column: x => x.RecipeDBrecipe_id,
                        principalTable: "RecipeDB",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingredient_RecipeFullWeb_RecipeFullWebrecipe_id",
                        column: x => x.RecipeFullWebrecipe_id,
                        principalTable: "RecipeFullWeb",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_RecipeDBrecipe_id",
                table: "Ingredient",
                column: "RecipeDBrecipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_RecipeFullWebrecipe_id",
                table: "Ingredient",
                column: "RecipeFullWebrecipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDB_itemInfoitemID",
                table: "ItemDB",
                column: "itemInfoitemID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRecipeHeaderAPI_ItemInfoWebitemID",
                table: "ItemRecipeHeaderAPI",
                column: "ItemInfoWebitemID");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListing_MarketDataDBitemID_MarketDataDBworldID",
                table: "MarketListing",
                columns: new[] { "MarketDataDBitemID", "MarketDataDBworldID" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDB_ItemDBitemID",
                table: "RecipeDB",
                column: "ItemDBitemID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDB_ItemInfoDBitemID",
                table: "RecipeDB",
                column: "ItemInfoDBitemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "ItemRecipeHeaderAPI");

            migrationBuilder.DropTable(
                name: "MarketListing");

            migrationBuilder.DropTable(
                name: "RecipeDB");

            migrationBuilder.DropTable(
                name: "RecipeFullWeb");

            migrationBuilder.DropTable(
                name: "ItemInfoWeb");

            migrationBuilder.DropTable(
                name: "MarketData");

            migrationBuilder.DropTable(
                name: "ItemDB");

            migrationBuilder.DropTable(
                name: "ItemInfoDB");
        }
    }
}
