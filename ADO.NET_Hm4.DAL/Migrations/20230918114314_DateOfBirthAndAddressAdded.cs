using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADO.NET_Hm4.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DateOfBirthAndAddressAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Students",
                nullable: true,
                defaultValueSql: "getdate()"); 

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Students",
                nullable: true); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
