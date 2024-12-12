using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Work_01.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salary = table.Column<decimal>(type: "money", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsaCurrentEmployee = table.Column<bool>(type: "bit", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    QualificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassingYear = table.Column<int>(type: "int", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.QualificationId);
                    table.ForeignKey(
                        name: "FK_Qualifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });
            //1
            string sql = @"CREATE PROCEDURE InsertQualification
                            @y  INT,
                            @d NVARCHAR(40),
                            @eid INT
                        AS
                            INSERT INTO Qualifications (PassingYear, Degree, EmployeeId) VALUES (@y, @d, @eid)
                        RETURN 0";
            migrationBuilder.Sql(sql);
            //2
            sql = @"CREATE PROCEDURE DeleteQualifications
                                @eid INT
                            AS
                                DELETE Qualifications WHERE EmployeeId = @eid
                            RETURN 0";
            migrationBuilder.Sql(sql);
            //3
            sql = @"CREATE PROCEDURE DeleteEmployee
                            @eid INT
                        AS
                            DELETE Employees WHERE EmployeeId = @eid
                        RETURN 0";
            migrationBuilder.Sql(sql);

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_EmployeeId",
                table: "Qualifications",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
