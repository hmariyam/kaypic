using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaypic_Web3.Migrations.MessagingDb
{
    /// <inheritdoc />
    public partial class InitMessaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: false),
                    Titre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_creation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationFichiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    TypeFichier = table.Column<int>(type: "int", nullable: false),
                    CreerParIdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationFichiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationFichiers_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationUtilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    Id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    ajouter_a = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationUtilisateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationUtilisateurs_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueAppels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_appelleur = table.Column<int>(type: "int", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    startedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueAppels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueAppels_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomEnvoyeur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdEnvoyeur = table.Column<int>(type: "int", nullable: false),
                    DateEnvoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueMessages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => new { x.MessageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Reactions_HistoriqueMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "HistoriqueMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationFichiers_ConversationId",
                table: "ConversationFichiers",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUtilisateurs_ConversationId",
                table: "ConversationUtilisateurs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueAppels_ConversationId",
                table: "HistoriqueAppels",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueMessages_ConversationId",
                table: "HistoriqueMessages",
                column: "ConversationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationFichiers");

            migrationBuilder.DropTable(
                name: "ConversationUtilisateurs");

            migrationBuilder.DropTable(
                name: "HistoriqueAppels");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "HistoriqueMessages");

            migrationBuilder.DropTable(
                name: "Conversations");
        }
    }
}
