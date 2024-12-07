using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbjjadMicroblogging.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class addUsertest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "Username" },
                values: new object[] { 1, "test@test.com", "user1", "$2a$11$JjcSOP8EdL56ajUMNbRRcuRS3ief5RFkv1W7aYqGGEfD1pMAWSOjW", "user1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
