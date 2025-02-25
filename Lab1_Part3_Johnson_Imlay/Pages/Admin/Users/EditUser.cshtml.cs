using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Users
{
    public class EditUserModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int UserID { get; set; }

        [BindProperty, Required]
        public string Username { get; set; } = "";

        [BindProperty]
        public string? Email { get; set; } = "";

        [BindProperty, Required]
        public string FirstName { get; set; } = "";

        [BindProperty, Required]
        public string LastName { get; set; } = "";

        [BindProperty, Required]
        public string UserType { get; set; } = "";

        [BindProperty]
        public string? Department { get; set; } = "";

        [BindProperty]
        public string? AdminType { get; set; } = "";

        [BindProperty]
        public int? BusinessPartnerID { get; set; } = null;

        public string Message { get; set; } = "";

        public List<BusinessPartner> BusinessPartners { get; set; } = new();

        public void OnGet(int id)
        {
            UserID = id;
            LoadUser();
            BusinessPartners = LoadBusinessPartners();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                BusinessPartners = LoadBusinessPartners();
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE [User] 
                                    SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName,
                                        UserType = @UserType, Department = @Department, AdminType = @AdminType, BusinessPartnerID = @BusinessPartnerID
                                    WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : Email);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(Department) ? DBNull.Value : Department);
                        cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(AdminType) ? DBNull.Value : AdminType);
                        cmd.Parameters.AddWithValue("@BusinessPartnerID", BusinessPartnerID.HasValue ? BusinessPartnerID.Value : DBNull.Value);

                        cmd.ExecuteNonQuery();
                        Message = "User updated successfully!";
                        return RedirectToPage("/Admin/Users/ViewUsers");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            BusinessPartners = LoadBusinessPartners();
            return Page();
        }

        private void LoadUser()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Username, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID FROM [User] WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Username = reader.GetString(0);
                            Email = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                            FirstName = reader.GetString(2);
                            LastName = reader.GetString(3);
                            UserType = reader.GetString(4);
                            Department = reader.IsDBNull(5) ? "N/A" : reader.GetString(5);
                            AdminType = reader.IsDBNull(6) ? "N/A" : reader.GetString(6);
                            BusinessPartnerID = reader.IsDBNull(7) ? null : reader.GetInt32(7);
                        }
                    }
                }
            }
        }

        private List<BusinessPartner> LoadBusinessPartners()
        {
            List<BusinessPartner> partners = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        partners.Add(new BusinessPartner { BusinessPartnerID = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                }
            }
            return partners;
        }

        public class BusinessPartner
        {
            public int BusinessPartnerID { get; set; }
            public string Name { get; set; } = "";
        }
        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();

            return Page();
        }
    }
}
