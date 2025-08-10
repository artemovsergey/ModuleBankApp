using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModuleBankApp.API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,");

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    InterestRate = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CounterPartyAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "public",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Accounts",
                columns: new[] { "Id", "Balance", "ClosedAt", "Currency", "InterestRate", "OwnerId", "Type" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 1500.00m, null, "USD", null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Checking" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 5000.00m, null, "EUR", 3.5m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Deposit" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), -250.00m, null, "USD", 15.0m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Credit" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 3200.50m, null, "GBP", null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Checking" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 10000.00m, null, "USD", 4.2m, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Deposit" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CounterPartyAccountId", "Currency", "Description", "Type" },
                values: new object[,]
                {
                    { new Guid("01010101-0101-0101-0101-010101010101"), new Guid("11111111-1111-1111-1111-111111111111"), 100.00m, new Guid("22222222-2222-2222-2222-222222222222"), "USD", "Перевод на депозитный счет", "Debit" },
                    { new Guid("02020202-0202-0202-0202-020202020202"), new Guid("22222222-2222-2222-2222-222222222222"), 50.00m, null, "EUR", "Пополнение через терминал", "Credit" },
                    { new Guid("03030303-0303-0303-0303-030303030303"), new Guid("33333333-3333-3333-3333-333333333333"), 200.00m, new Guid("44444444-4444-4444-4444-444444444444"), "USD", "Погашение кредита", "Credit" },
                    { new Guid("04040404-0404-0404-0404-040404040404"), new Guid("44444444-4444-4444-4444-444444444444"), 75.50m, null, "GBP", "Оплата услуг", "Debit" },
                    { new Guid("05050505-0505-0505-0505-050505050505"), new Guid("55555555-5555-5555-5555-555555555555"), 500.00m, null, "USD", "Начисление процентов", "Credit" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_OwnerId_Hash",
                schema: "public",
                table: "Accounts",
                column: "OwnerId")
                .Annotation("Npgsql:IndexMethod", "HASH");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Type_Currency",
                schema: "public",
                table: "Accounts",
                columns: new[] { "Type", "Currency" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId_CreatedAt",
                schema: "public",
                table: "Transactions",
                columns: new[] { "AccountId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreatedAt_Gist",
                schema: "public",
                table: "Transactions",
                column: "CreatedAt")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Type_Currency",
                schema: "public",
                table: "Transactions",
                columns: new[] { "Type", "Currency" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "public");
        }
    }
}
