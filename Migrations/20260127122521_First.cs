using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MITCRMS.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TutorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HodId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdminId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SuperAdminId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BursarId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SuperAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuperAdmins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Bursars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bursars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bursars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DepartmentName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartmentCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HodId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Hods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DepartmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hods_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hods_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tutors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DepartmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tutors_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tutors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Tittle = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TutorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    HodId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AdminId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    BursarId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ReportDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Bursars_BursarId",
                        column: x => x.BursarId,
                        principalTable: "Bursars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Hods_HodId",
                        column: x => x.HodId,
                        principalTable: "Hods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutors",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "DateCreated", "RoleName" },
                values: new object[,]
                {
                    { new Guid("6e3d4978-dcb0-42ea-9c48-7f6209d4a856"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Tutor" },
                    { new Guid("6e3d4978-dcb0-42ea-9c48-7f6209d4a871"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Bursar" },
                    { new Guid("6e3d4978-dcb0-42ea-9c48-7f6209d4a882"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Admin" },
                    { new Guid("a45c9e02-1f0b-4e57-b3d8-9b77b4a302be"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Hod" },
                    { new Guid("d2719e67-52f4-4f9c-bdb2-123456789abc"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "SuperAdmin" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AdminId", "BursarId", "DateCreated", "Email", "EmailConfirmed", "HodId", "PasswordHash", "SuperAdminId", "TutorId" },
                values: new object[] { new Guid("c8f2e5ab-9f34-4b97-8b7c-1a5e86c77e42"), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Admin001@gmail.com", true, new Guid("00000000-0000-0000-0000-000000000000"), "AQAAAAIAAYagAAAAEJjieFsJGM2Xgr+WpuS3juOABbBCvbqSvpym4WzP/SDMuvGz6qH+EFgm19l8SUHUGA==", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "SuperAdmins",
                columns: new[] { "Id", "Address", "DateCreated", "DateOfBirth", "FirstName", "LastName", "PhoneNumber", "UserId" },
                values: new object[] { new Guid("f0e25b73-7d1a-4c19-8b2f-09a3efb40d12"), "Ogun State", new DateTime(2025, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1997, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc), "SuperAdmin", "Mitcrms", "+23470456780", new Guid("c8f2e5ab-9f34-4b97-8b7c-1a5e86c77e42") });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "DateCreated", "RoleId", "UserId" },
                values: new object[] { new Guid("7ad9b1e1-4c23-46a2-b8e4-219ab417f71f"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d2719e67-52f4-4f9c-bdb2-123456789abc"), new Guid("c8f2e5ab-9f34-4b97-8b7c-1a5e86c77e42") });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_DepartmentId",
                table: "Admins",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bursars_DepartmentId",
                table: "Bursars",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bursars_UserId",
                table: "Bursars",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HodId",
                table: "Departments",
                column: "HodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hods_DepartmentId",
                table: "Hods",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Hods_UserId",
                table: "Hods",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AdminId",
                table: "Reports",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_BursarId",
                table: "Reports",
                column: "BursarId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_DepartmentId",
                table: "Reports",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_HodId",
                table: "Reports",
                column: "HodId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TutorId",
                table: "Reports",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdmins_UserId",
                table: "SuperAdmins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutors_DepartmentId",
                table: "Tutors",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tutors_UserId",
                table: "Tutors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Departments_DepartmentId",
                table: "Admins",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bursars_Departments_DepartmentId",
                table: "Bursars",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Hods_HodId",
                table: "Departments",
                column: "HodId",
                principalTable: "Hods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hods_Departments_DepartmentId",
                table: "Hods");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "SuperAdmins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Bursars");

            migrationBuilder.DropTable(
                name: "Tutors");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Hods");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
