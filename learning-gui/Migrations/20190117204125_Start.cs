using Microsoft.EntityFrameworkCore.Migrations;

namespace learning_gui.Migrations
{
    public partial class Start : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Category",
                table => new
                {
                    CategoryId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Number = table.Column<int>(),
                    CategoryIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Category", x => x.CategoryId); });

            migrationBuilder.CreateTable(
                "Genders",
                table => new
                {
                    GenderId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    GenderCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Genders", x => x.GenderId); });

            migrationBuilder.CreateTable(
                "Lemmas",
                table => new
                {
                    LemmaId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaText = table.Column<string>(nullable: true),
                    LemmaShortDef = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Lemmas", x => x.LemmaId); });

            migrationBuilder.CreateTable(
                "PartOfSpeech",
                table => new
                {
                    PartId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    PartName = table.Column<string>(nullable: true),
                    PartDesc = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_PartOfSpeech", x => x.PartId); });

            migrationBuilder.CreateTable(
                "Definitions",
                table => new
                {
                    DefinitionId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaId = table.Column<int>(),
                    Data = table.Column<string>(nullable: true),
                    Level = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.DefinitionId);
                    table.ForeignKey(
                        "FK_Definitions_Lemmas_LemmaId",
                        x => x.LemmaId,
                        "Lemmas",
                        "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Forms",
                table => new
                {
                    FormId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaId = table.Column<int>(),
                    MorphCode = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    MiscFeatures = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                    table.ForeignKey(
                        "FK_Forms_Lemmas_LemmaId",
                        x => x.LemmaId,
                        "Lemmas",
                        "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserLearntWords",
                table => new
                {
                    UserLearntWordId = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    LemmaId = table.Column<int>(),
                    RevisionStage = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLearntWords", x => x.UserLearntWordId);
                    table.ForeignKey(
                        "FK_UserLearntWords_Lemmas_LemmaId",
                        x => x.LemmaId,
                        "Lemmas",
                        "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LemmaData",
                table => new
                {
                    LemmaId = table.Column<int>(),
                    PartOfSpeechId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    GenderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LemmaData", x => x.LemmaId);
                    table.ForeignKey(
                        "FK_LemmaData_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_LemmaData_Genders_GenderId",
                        x => x.GenderId,
                        "Genders",
                        "GenderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_LemmaData_Lemmas_LemmaId",
                        x => x.LemmaId,
                        "Lemmas",
                        "LemmaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_LemmaData_PartOfSpeech_PartOfSpeechId",
                        x => x.PartOfSpeechId,
                        "PartOfSpeech",
                        "PartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Definitions_LemmaId",
                "Definitions",
                "LemmaId");

            migrationBuilder.CreateIndex(
                "IX_Forms_LemmaId",
                "Forms",
                "LemmaId");

            migrationBuilder.CreateIndex(
                "IX_LemmaData_CategoryId",
                "LemmaData",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_LemmaData_GenderId",
                "LemmaData",
                "GenderId");

            migrationBuilder.CreateIndex(
                "IX_LemmaData_PartOfSpeechId",
                "LemmaData",
                "PartOfSpeechId");

            migrationBuilder.CreateIndex(
                "IX_UserLearntWords_LemmaId",
                "UserLearntWords",
                "LemmaId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Definitions");

            migrationBuilder.DropTable(
                "Forms");

            migrationBuilder.DropTable(
                "LemmaData");

            migrationBuilder.DropTable(
                "UserLearntWords");

            migrationBuilder.DropTable(
                "Category");

            migrationBuilder.DropTable(
                "Genders");

            migrationBuilder.DropTable(
                "PartOfSpeech");

            migrationBuilder.DropTable(
                "Lemmas");
        }
    }
}