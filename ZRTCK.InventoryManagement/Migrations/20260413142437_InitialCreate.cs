using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZRTCK.InventoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    order_id = table.Column<int>(type: "INTEGER", nullable: true),
                    order_date = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assignments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    invoice_id = table.Column<int>(type: "INTEGER", nullable: false),
                    invoice_date = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    rank = table.Column<string>(type: "TEXT", nullable: false),
                    position = table.Column<string>(type: "TEXT", nullable: false),
                    first_name = table.Column<string>(type: "TEXT", nullable: false),
                    last_name = table.Column<string>(type: "TEXT", nullable: false),
                    is_obsolete = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_people", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "category_fields",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    category_id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    code = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    order = table.Column<int>(type: "INTEGER", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_fields", x => x.id);
                    table.ForeignKey(
                        name: "fk_category_fields_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    product_code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    price = table.Column<double>(type: "REAL", nullable: false),
                    category_id = table.Column<string>(type: "TEXT", nullable: true),
                    factory_id = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    inventory_invoice_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_inventory_invoices_inventory_invoice_id",
                        column: x => x.inventory_invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_assignment_row",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    inventory_assignment_id = table.Column<int>(type: "INTEGER", nullable: false),
                    person_id = table.Column<int>(type: "INTEGER", nullable: false),
                    inventory_item_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_assignment_row", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_assignment_row_assignments_inventory_assignment_id",
                        column: x => x.inventory_assignment_id,
                        principalTable: "assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventory_assignment_row_inventory_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "inventory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventory_assignment_row_people_person_id",
                        column: x => x.person_id,
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_fields",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    item_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    category_field_id = table.Column<int>(type: "INTEGER", nullable: false),
                    value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_fields", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_fields_category_fields_category_field_id",
                        column: x => x.category_field_id,
                        principalTable: "category_fields",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_fields_inventory_item_id",
                        column: x => x.item_id,
                        principalTable: "inventory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_category_fields_category_id",
                table: "category_fields",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_category_id",
                table: "inventory",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_inventory_invoice_id",
                table: "inventory",
                column: "inventory_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_assignment_row_inventory_assignment_id",
                table: "inventory_assignment_row",
                column: "inventory_assignment_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_assignment_row_inventory_item_id",
                table: "inventory_assignment_row",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_assignment_row_person_id",
                table: "inventory_assignment_row",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_fields_category_field_id",
                table: "item_fields",
                column: "category_field_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_fields_item_id",
                table: "item_fields",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_assignment_row");

            migrationBuilder.DropTable(
                name: "item_fields");

            migrationBuilder.DropTable(
                name: "assignments");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropTable(
                name: "category_fields");

            migrationBuilder.DropTable(
                name: "inventory");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "invoices");
        }
    }
}
