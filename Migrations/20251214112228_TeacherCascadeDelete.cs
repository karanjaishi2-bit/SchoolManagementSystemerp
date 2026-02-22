using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Migrations
{
    /// <inheritdoc />
    public partial class TeacherCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "BillingMasters");

            migrationBuilder.RenameColumn(
                name: "BillingDate",
                table: "BillingMasters",
                newName: "BillDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BillDate",
                table: "BillingMasters",
                newName: "BillingDate");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "BillingMasters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
