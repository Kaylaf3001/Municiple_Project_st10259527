using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Municiple_Project_st10259527.Migrations
{
    /// <inheritdoc />
    public partial class ConvertStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a temporary column to store the converted status
            migrationBuilder.AddColumn<int>(
                name: "TempStatus",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Convert string status to enum values
            migrationBuilder.Sql(@"
                UPDATE Reports 
                SET TempStatus = 
                    CASE 
                        WHEN Status = 'Pending' THEN 0
                        WHEN Status = 'InReview' THEN 1
                        WHEN Status = 'Approved' THEN 2
                        WHEN Status = 'Rejected' THEN 3
                        WHEN Status = 'Completed' THEN 4
                        ELSE 0
                    END");

            // Drop the old Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");

            // Rename the temporary column to Status
            migrationBuilder.RenameColumn(
                name: "TempStatus",
                table: "Reports",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back the string Status column
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Convert enum values back to strings
            migrationBuilder.Sql(@"
                UPDATE Reports 
                SET Status = 
                    CASE 
                        WHEN Status = 0 THEN 'Pending'
                        WHEN Status = 1 THEN 'InReview'
                        WHEN Status = 2 THEN 'Approved'
                        WHEN Status = 3 THEN 'Rejected'
                        WHEN Status = 4 THEN 'Completed'
                        ELSE 'Pending'
                    END");

            // Remove the temporary column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Reports",
                newName: "TempStatus");
        }
    }
}
