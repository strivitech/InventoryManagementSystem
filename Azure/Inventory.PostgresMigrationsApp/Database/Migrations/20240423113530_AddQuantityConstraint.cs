﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.PostgresMigrationsApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Quantity",
                table: "Products",
                sql: "\"Quantity\" >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Quantity",
                table: "Products");
        }
    }
}
