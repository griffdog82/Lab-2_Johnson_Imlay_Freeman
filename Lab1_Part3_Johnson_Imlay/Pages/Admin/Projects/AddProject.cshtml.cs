using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Projects
{
    public class AddProjectModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty, Required]
        public string Title { get; set; } = "";

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required]
        public int CreatedBy { get; set; }

        [BindProperty]
        public int? BusinessPartnerID { get; set; } = null;

        [BindProperty]
        public int? GrantID { get; set; } = null;

        [BindProperty]
        public int? AssignedFacultyID { get; set; } = null; // FIXED: Faculty Member property

        [BindProperty]
        public int AssigningAdminID { get; set; } = 1; // FIXED: Simulated Admin ID for now

        public string Message { get; set; } = "";

        public List<UserModel> Users { get; set; } = new();
        public List<BusinessPartner> BusinessPartners { get; set; } = new();
        public List<GrantModel> Grants { get; set; } = new();
        public List<UserModel> FacultyMembers { get; set; } = new(); // FIXED: Faculty members list

        public void OnGet()
        {
            Users = LoadUsers();
            BusinessPartners = LoadBusinessPartners();
            Grants = LoadGrants();
            FacultyMembers = LoadFacultyMembers(); // FIXED: Load Faculty Members
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Users = LoadUsers();
                BusinessPartners = LoadBusinessPartners();
                Grants = LoadGrants();
                FacultyMembers = LoadFacultyMembers();
                return Page();
            }

            int projectID;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Project 
                                    (Title, DueDate, CreatedBy, BusinessPartnerID, GrantID) 
                                    OUTPUT INSERTED.ProjectID
                                    VALUES (@Title, @DueDate, @CreatedBy, @BusinessPartnerID, @GrantID)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", Title);
                        cmd.Parameters.AddWithValue("@DueDate", DueDate);
                        cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                        cmd.Parameters.AddWithValue("@BusinessPartnerID", BusinessPartnerID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@GrantID", GrantID ?? (object)DBNull.Value);

                        projectID = (int)cmd.ExecuteScalar();
                    }

                    if (AssignedFacultyID.HasValue)
                    {
                        string assignQuery = @"INSERT INTO ProjectAssignment 
                                               (ProjectID, UserID, Role) 
                                               VALUES (@ProjectID, @UserID, 'Faculty Member')";

                        using (SqlCommand assignCmd = new SqlCommand(assignQuery, conn))
                        {
                            assignCmd.Parameters.AddWithValue("@ProjectID", projectID);
                            assignCmd.Parameters.AddWithValue("@UserID", AssignedFacultyID.Value);
                            assignCmd.ExecuteNonQuery();
                        }
                    }
                }

                Message = "Project added successfully!";
                return RedirectToPage("/Admin/Projects/ProjectList");
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            Users = LoadUsers();
            BusinessPartners = LoadBusinessPartners();
            Grants = LoadGrants();
            FacultyMembers = LoadFacultyMembers();
            return Page();
        }

        private List<UserModel> LoadUsers()
        {
            List<UserModel> users = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName, LastName FROM [User]", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1) + " " + reader.GetString(2)
                        });
                    }
                }
            }
            return users;
        }

        private List<UserModel> LoadFacultyMembers()
        {
            List<UserModel> faculty = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName, LastName FROM [User] WHERE UserType = 'Faculty'", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        faculty.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1) + " " + reader.GetString(2)
                        });
                    }
                }
            }
            return faculty;
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

        private List<GrantModel> LoadGrants()
        {
            List<GrantModel> grants = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT GrantID, FundingSource, Amount FROM [Grant]", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        grants.Add(new GrantModel
                        {
                            GrantID = reader.GetInt32(0),
                            FundingSource = reader.GetString(1),
                            Amount = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return grants;
        }

        public class UserModel { public int UserID { get; set; } public string FullName { get; set; } = ""; }
        public class BusinessPartner { public int BusinessPartnerID { get; set; } public string Name { get; set; } = ""; }
        public class GrantModel { public int GrantID { get; set; } public string FundingSource { get; set; } = ""; public decimal Amount { get; set; } }
    }
}
