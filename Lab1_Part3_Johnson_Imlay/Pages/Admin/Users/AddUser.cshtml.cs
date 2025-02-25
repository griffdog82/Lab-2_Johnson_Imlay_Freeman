using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Users
{
    public class AddUserModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty, Required]
        public string Username { get; set; } = "";

        [BindProperty, Required]
        public string Password { get; set; } = "";

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

        public void OnGet()
        {
            BusinessPartners = LoadBusinessPartners();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                BusinessPartners = LoadBusinessPartners();
                return Page();
            }///This if statement checks to ensure that the model state is valid. If it is not, the page is reloaded with the business partners reloaded as well.

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO [User] 
                                    (Username, Password, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID) 
                                    VALUES (@Username, @Password, @Email, @FirstName, @LastName, @UserType, @Department, @AdminType, @BusinessPartnerID)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : Email);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(Department) ? DBNull.Value : Department);
                        cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(AdminType) ? DBNull.Value : AdminType);
                        cmd.Parameters.AddWithValue("@BusinessPartnerID", BusinessPartnerID.HasValue ? BusinessPartnerID.Value : DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Message = "User added successfully!";
                            return RedirectToPage("/Admin/Users/ViewUsers");
                        }
                        else
                        {
                            Message = "Error adding user.";
                        }
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
                        partners.Add(new BusinessPartner
                        {
                            BusinessPartnerID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
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
            ModelState.Clear(); // Clear validation state

            Username = "testuser";
            Password = "Password123";
            Email = "test@example.com";
            FirstName = "Test";
            LastName = "User";
            UserType = "RepOfBusiness"; // Setting this to ensure BusinessPartnerID is used
            AdminType = "Super Admin"; // Example Admin Type
            Department = "Engineering"; // Example Department

            // Reload business partners so dropdown doesn't break
            BusinessPartners = LoadBusinessPartners();

            // Ensure BusinessPartnerID is set properly if "RepOfBusiness"
            if (UserType == "RepOfBusiness" && BusinessPartners.Count > 0)
            {
                BusinessPartnerID = BusinessPartners[0].BusinessPartnerID;
            }
            else
            {
                BusinessPartnerID = null;
            }

            return Page();
        }


    }
}
