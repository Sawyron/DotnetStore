using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    parent_id = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_categories_categories_parent_temp_id",
                        column: x => x.parent_id,
                        principalTable: "categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    path = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    category_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_tags_categories_category_id1",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    price = table.Column<int>(type: "INTEGER", nullable: false),
                    description = table.Column<string>(type: "VARCHAR(512)", maxLength: 512, nullable: false),
                    photo_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    category_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_categories_category_id1",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_products_files_photo_id1",
                        column: x => x.photo_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tag_options",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    value = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    tag_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_tag_options_tag_data_tag_temp_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_tag_options",
                columns: table => new
                {
                    options_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    product_data_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_tag_options", x => new { x.options_id, x.product_data_id });
                    table.ForeignKey(
                        name: "fk_product_tag_options_products_product_data_id",
                        column: x => x.product_data_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_product_tag_options_tag_option_data_options_temp_id",
                        column: x => x.options_id,
                        principalTable: "tag_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_id",
                table: "categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_path",
                table: "files",
                column: "path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_tag_options_product_data_id",
                table: "product_tag_options",
                column: "product_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id_name",
                table: "products",
                columns: new[] { "category_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_photo_id",
                table: "products",
                column: "photo_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tag_options_tag_id_value",
                table: "tag_options",
                columns: new[] { "tag_id", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_category_id_name",
                table: "tags",
                columns: new[] { "category_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_tag_options");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "tag_options");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
