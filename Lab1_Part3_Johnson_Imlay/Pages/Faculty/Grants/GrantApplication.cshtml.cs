using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Faculty.Grants
{
    public class GrantApplication : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty, Required]
        public string Category { get; set; } = "";

        [BindProperty, Required]
        public string GrantName { get; set; } = "";

        [BindProperty, Required]
        public string FundingSource { get; set; } = "";

        [BindProperty, Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime? AwardDate { get; set; }

        [BindProperty, Required]
        public decimal Amount { get; set; }

        [BindProperty, Required]
        public string? LeadFacultyID { get; set; } // Now a string? to match SQL change

        [BindProperty]
        public string? BusinessPartnerID { get; set; } // Now a string? to match SQL change

        public List<SelectListItem> FacultyMembers { get; set; } = new();
        public List<SelectListItem> BusinessPartners { get; set; } = new();

        public string Message { get; set; } = "";

        public void OnGet()
        {
            // Load Faculty Members
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'Faculty'", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FacultyMembers.Add(new SelectListItem
                        {
                            Value = reader["UserID"].ToString(),
                            Text = reader["FullName"].ToString()
                        });
                    }
                }

                // Load Business Partner Representatives
                using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BusinessPartners.Add(new SelectListItem
                        {
                            Value = reader["BusinessPartnerID"].ToString(),
                            Text = reader["Name"].ToString()
                        });
                    }
                }
            }
        }

        public IActionResult OnPost([FromForm] string action)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string status = action == "submit" ? "Submitted" : "Draft";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO [Grant] 
                                    (Category, GrantName, FundingSource, SubmissionDate, AwardDate, Amount, LeadFacultyID, BusinessPartnerID, Status)
                                    VALUES (@Category, @GrantName, @FundingSource, @SubmissionDate, @AwardDate, @Amount, @LeadFacultyID, @BusinessPartnerID, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Category", Category);
                        cmd.Parameters.AddWithValue("@GrantName", GrantName);
                        cmd.Parameters.AddWithValue("@FundingSource", FundingSource);
                        cmd.Parameters.AddWithValue("@SubmissionDate", SubmissionDate);
                        cmd.Parameters.AddWithValue("@AwardDate", AwardDate.HasValue ? (object)AwardDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@LeadFacultyID", LeadFacultyID);
                        cmd.Parameters.AddWithValue("@BusinessPartnerID",
                            string.IsNullOrWhiteSpace(BusinessPartnerID) ? DBNull.Value : BusinessPartnerID);
                        cmd.Parameters.AddWithValue("@Status", status);

                        cmd.ExecuteNonQuery();
                    }
                }

                Message = action == "submit" ? "Application submitted successfully!" : "Draft saved successfully!";
                return RedirectToPage("/Faculty/FacultyDashboard");
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
                return Page();
            }
        }
    }
}





