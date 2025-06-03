using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EloDrinksAPI.Migrations
{
    /// <inheritdoc />
    public partial class adicionandoCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_Orcamento_Usuario1",
                table: "orcamento");

            migrationBuilder.DropForeignKey(
                name: "fk_Orcamento_has_Item_Item1",
                table: "orcamento_has_item");

            migrationBuilder.DropForeignKey(
                name: "fk_Orcamento_has_Item_Orcamento1",
                table: "orcamento_has_item");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetTokens_usuario_UserId",
                table: "PasswordResetTokens");

            migrationBuilder.DropForeignKey(
                name: "fk_Pedido_Orcamento1",
                table: "pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetTokens",
                table: "PasswordResetTokens");

            migrationBuilder.RenameTable(
                name: "PasswordResetTokens",
                newName: "passwordresettokens");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "passwordresettokens",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "passwordresettokens",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "passwordresettokens",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "passwordresettokens",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "cep",
                table: "orcamento",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "passwordresettokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_Orcamento_Usuario1",
                table: "orcamento",
                column: "Usuario_idUsuario",
                principalTable: "usuario",
                principalColumn: "idUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_Orcamento_has_Item_Item1",
                table: "orcamento_has_item",
                column: "Item_idItem",
                principalTable: "item",
                principalColumn: "idItem",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_Orcamento_has_Item_Orcamento1",
                table: "orcamento_has_item",
                columns: new[] { "Orcamento_idOrcamento", "Orcamento_Usuario_idUsuario" },
                principalTable: "orcamento",
                principalColumns: new[] { "idOrcamento", "Usuario_idUsuario" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "passwordresettokens_ibfk_1",
                table: "passwordresettokens",
                column: "UserId",
                principalTable: "usuario",
                principalColumn: "idUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_Pedido_Orcamento1",
                table: "pedido",
                columns: new[] { "Orcamento_idOrcamento", "Orcamento_Usuario_idUsuario" },
                principalTable: "orcamento",
                principalColumns: new[] { "idOrcamento", "Usuario_idUsuario" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_Orcamento_Usuario1",
                table: "orcamento");

            migrationBuilder.DropForeignKey(
                name: "fk_Orcamento_has_Item_Item1",
                table: "orcamento_has_item");

            migrationBuilder.DropForeignKey(
                name: "fk_Orcamento_has_Item_Orcamento1",
                table: "orcamento_has_item");

            migrationBuilder.DropForeignKey(
                name: "passwordresettokens_ibfk_1",
                table: "passwordresettokens");

            migrationBuilder.DropForeignKey(
                name: "fk_Pedido_Orcamento1",
                table: "pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "passwordresettokens");

            migrationBuilder.RenameTable(
                name: "passwordresettokens",
                newName: "PasswordResetTokens");

            migrationBuilder.RenameIndex(
                name: "UserId",
                table: "PasswordResetTokens",
                newName: "IX_PasswordResetTokens_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PasswordResetTokens",
                type: "varchar(45)",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "PasswordResetTokens",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "PasswordResetTokens",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "cep",
                table: "orcamento",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetTokens",
                table: "PasswordResetTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_Orcamento_Usuario1",
                table: "orcamento",
                column: "Usuario_idUsuario",
                principalTable: "usuario",
                principalColumn: "idUsuario");

            migrationBuilder.AddForeignKey(
                name: "fk_Orcamento_has_Item_Item1",
                table: "orcamento_has_item",
                column: "Item_idItem",
                principalTable: "item",
                principalColumn: "idItem");

            migrationBuilder.AddForeignKey(
                name: "fk_Orcamento_has_Item_Orcamento1",
                table: "orcamento_has_item",
                columns: new[] { "Orcamento_idOrcamento", "Orcamento_Usuario_idUsuario" },
                principalTable: "orcamento",
                principalColumns: new[] { "idOrcamento", "Usuario_idUsuario" });

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetTokens_usuario_UserId",
                table: "PasswordResetTokens",
                column: "UserId",
                principalTable: "usuario",
                principalColumn: "idUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_Pedido_Orcamento1",
                table: "pedido",
                columns: new[] { "Orcamento_idOrcamento", "Orcamento_Usuario_idUsuario" },
                principalTable: "orcamento",
                principalColumns: new[] { "idOrcamento", "Usuario_idUsuario" });
        }
    }
}
