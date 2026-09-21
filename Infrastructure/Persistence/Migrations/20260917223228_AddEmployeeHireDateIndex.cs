using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayRollApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeHireDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employees_HireDate",
                schema: "payroll",
                table: "Employees",
                column: "HireDate",
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_HireDate",
                schema: "payroll",
                table: "Employees");
        }
    }
}
