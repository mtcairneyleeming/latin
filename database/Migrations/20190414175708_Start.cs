using Microsoft.EntityFrameworkCore.Migrations;

namespace database.Migrations
{
    public partial class Start : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: false),
                    CategoryIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    GenderId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GenderCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.GenderId);
                });

            migrationBuilder.CreateTable(
                name: "Lemmas",
                columns: table => new
                {
                    LemmaId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaText = table.Column<string>(nullable: true),
                    LemmaShortDef = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lemmas", x => x.LemmaId);
                });

            migrationBuilder.CreateTable(
                name: "PartOfSpeech",
                columns: table => new
                {
                    PartId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartName = table.Column<string>(nullable: true),
                    PartDesc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOfSpeech", x => x.PartId);
                });

            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    DefinitionId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaId = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.DefinitionId);
                    table.ForeignKey(
                        name: "FK_Definitions_Lemmas_LemmaId",
                        column: x => x.LemmaId,
                        principalTable: "Lemmas",
                        principalColumn: "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    FormId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaId = table.Column<int>(nullable: false),
                    MorphCode = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    MiscFeatures = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_Forms_Lemmas_LemmaId",
                        column: x => x.LemmaId,
                        principalTable: "Lemmas",
                        principalColumn: "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLearntWords",
                columns: table => new
                {
                    UserLearntWordId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaId = table.Column<int>(nullable: false),
                    RevisionStage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLearntWords", x => x.UserLearntWordId);
                    table.ForeignKey(
                        name: "FK_UserLearntWords_Lemmas_LemmaId",
                        column: x => x.LemmaId,
                        principalTable: "Lemmas",
                        principalColumn: "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LemmaData",
                columns: table => new
                {
                    LemmaId = table.Column<int>(nullable: false),
                    PartOfSpeechId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    GenderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LemmaData", x => x.LemmaId);
                    table.ForeignKey(
                        name: "FK_LemmaData_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LemmaData_Genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Genders",
                        principalColumn: "GenderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LemmaData_Lemmas_LemmaId",
                        column: x => x.LemmaId,
                        principalTable: "Lemmas",
                        principalColumn: "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LemmaData_PartOfSpeech_PartOfSpeechId",
                        column: x => x.PartOfSpeechId,
                        principalTable: "PartOfSpeech",
                        principalColumn: "PartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Definitions_LemmaId",
                table: "Definitions",
                column: "LemmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_LemmaId",
                table: "Forms",
                column: "LemmaId");

            migrationBuilder.CreateIndex(
                name: "IX_LemmaData_CategoryId",
                table: "LemmaData",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LemmaData_GenderId",
                table: "LemmaData",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_LemmaData_PartOfSpeechId",
                table: "LemmaData",
                column: "PartOfSpeechId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLearntWords_LemmaId",
                table: "UserLearntWords",
                column: "LemmaId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Definitions");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "LemmaData");

            migrationBuilder.DropTable(
                name: "UserLearntWords");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "PartOfSpeech");

            migrationBuilder.DropTable(
                name: "Lemmas");
        }
    }
}
