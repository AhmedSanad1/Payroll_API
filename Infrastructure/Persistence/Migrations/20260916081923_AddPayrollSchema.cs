using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PayRollApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payroll");

            migrationBuilder.CreateTable(
                name: "AttendanceRules",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromDays = table.Column<byte>(type: "tinyint", nullable: false),
                    ToDays = table.Column<byte>(type: "tinyint", nullable: true),
                    AdjustmentType = table.Column<byte>(type: "tinyint", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IncentivePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobGrades",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRuns",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodYear = table.Column<short>(type: "smallint", nullable: false),
                    PeriodMonth = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CalculationMode = table.Column<byte>(type: "tinyint", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollSettings",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CalculationMode = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollSettings", x => x.Id);
                    table.CheckConstraint("CK_PayrollSettings_SingleRow", "[Id] = 1");
                });

            migrationBuilder.CreateTable(
                name: "ServiceIncentiveTiers",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinYearsExceeded = table.Column<byte>(type: "tinyint", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceIncentiveTiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    JobGradeId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "payroll",
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_JobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalSchema: "payroll",
                        principalTable: "JobGrades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAbsences",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AbsenceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAbsences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAbsences_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "payroll",
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PayrollItems",
                schema: "payroll",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollRunId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JobGradeId = table.Column<int>(type: "int", nullable: false),
                    GradeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeptIncentivePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DeptIncentiveAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ServiceYears = table.Column<byte>(type: "tinyint", nullable: false),
                    ServiceIncentivePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ServiceIncentiveAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AbsentDays = table.Column<byte>(type: "tinyint", nullable: false),
                    AttendanceAdjustmentType = table.Column<byte>(type: "tinyint", nullable: true),
                    AttendancePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AttendanceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollItems_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "payroll",
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollItems_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "payroll",
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollItems_JobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalSchema: "payroll",
                        principalTable: "JobGrades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollItems_PayrollRuns_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalSchema: "payroll",
                        principalTable: "PayrollRuns",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "payroll",
                table: "AttendanceRules",
                columns: new[] { "Id", "AdjustmentType", "CreatedAt", "CreatedBy", "FromDays", "LastModifiedAt", "LastModifiedBy", "Percent", "ToDays" },
                values: new object[,]
                {
                    { 1, (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (byte)0, null, null, 5m, (byte)0 },
                    { 2, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (byte)3, null, null, 5m, (byte)5 },
                    { 3, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (byte)6, null, null, 10m, (byte)10 },
                    { 4, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (byte)11, null, null, 20m, (byte)15 },
                    { 5, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (byte)16, null, null, 30m, null }
                });

            migrationBuilder.InsertData(
                schema: "payroll",
                table: "JobGrades",
                columns: new[] { "Id", "BaseSalary", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, 12000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "أولى", "First" },
                    { 2, 9000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "ثانية", "Second" },
                    { 3, 6500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "ثالثة", "Third" }
                });

            migrationBuilder.InsertData(
                schema: "payroll",
                table: "PayrollSettings",
                columns: new[] { "Id", "CalculationMode", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy" },
                values: new object[] { 1, (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null });

            migrationBuilder.InsertData(
                schema: "payroll",
                table: "ServiceIncentiveTiers",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy", "MinYearsExceeded", "Percent" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, (byte)5, 5m },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, (byte)7, 10m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                schema: "payroll",
                table: "Departments",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAbsences_AbsenceDate",
                schema: "payroll",
                table: "EmployeeAbsences",
                column: "AbsenceDate")
                .Annotation("SqlServer:Include", new[] { "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAbsences_EmployeeId_AbsenceDate",
                schema: "payroll",
                table: "EmployeeAbsences",
                columns: new[] { "EmployeeId", "AbsenceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                schema: "payroll",
                table: "Employees",
                column: "DepartmentId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                schema: "payroll",
                table: "Employees",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FullName",
                schema: "payroll",
                table: "Employees",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobGradeId",
                schema: "payroll",
                table: "Employees",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_DepartmentId",
                schema: "payroll",
                table: "PayrollItems",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_EmployeeId",
                schema: "payroll",
                table: "PayrollItems",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_JobGradeId",
                schema: "payroll",
                table: "PayrollItems",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_PayrollRunId_DepartmentId",
                schema: "payroll",
                table: "PayrollItems",
                columns: new[] { "PayrollRunId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_PayrollRunId_EmployeeId",
                schema: "payroll",
                table: "PayrollItems",
                columns: new[] { "PayrollRunId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRuns_PeriodYear_PeriodMonth",
                schema: "payroll",
                table: "PayrollRuns",
                columns: new[] { "PeriodYear", "PeriodMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIncentiveTiers_MinYearsExceeded",
                schema: "payroll",
                table: "ServiceIncentiveTiers",
                column: "MinYearsExceeded",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRules",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "EmployeeAbsences",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "PayrollItems",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "PayrollSettings",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "ServiceIncentiveTiers",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "PayrollRuns",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "payroll");

            migrationBuilder.DropTable(
                name: "JobGrades",
                schema: "payroll");
        }
    }
}
