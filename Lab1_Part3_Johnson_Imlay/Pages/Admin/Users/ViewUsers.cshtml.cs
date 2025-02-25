using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Users
{
    public class ViewUsersModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        public List<UserModel> Users { get; set; } = new();

        public void OnGet()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT u.UserID, u.Username, u.Email, u.FirstName, u.LastName, u.UserType, 
                           u.Department, u.AdminType, b.Name AS BusinessPartnerName
                    FROM [User] u
                    LEFT JOIN BusinessPartner b ON u.BusinessPartnerID = b.BusinessPartnerID", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.IsDBNull(2) ? "N/A" : reader.GetString(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            UserType = reader.GetString(5),
                            Department = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
                            AdminType = reader.IsDBNull(7) ? "N/A" : reader.GetString(7),
                            BusinessPartnerName = reader.IsDBNull(8) ? "N/A" : reader.GetString(8)
                        });
                    }
                }
            }
        }

        public class UserModel
        {
            public int UserID { get; set; }
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string UserType { get; set; } = "";
            public string? Department { get; set; }
            public string? AdminType { get; set; }
            public string? BusinessPartnerName { get; set; }
        }
    }
}
