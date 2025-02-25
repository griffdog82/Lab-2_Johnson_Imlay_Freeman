using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        public List<Grant> Grants { get; set; } = new();
        public List<Project> Projects { get; set; } = new();

        public void OnGet()
        {
            LoadGrants();
            LoadProjects();
        }

        private void LoadGrants()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT g.GrantID, g.GrantName, g.Category, g.FundingSource, g.SubmissionDate, g.AwardDate, 
                           g.Amount, g.Status, u.FirstName + ' ' + u.LastName AS LeadFacultyName
                    FROM [Grant] g
                    JOIN [User] u ON g.LeadFacultyID = u.UserID", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Grants.Add(new Grant
                        {
                            GrantID = reader.GetInt32(0),
                            GrantName = reader.GetString(1),
                            Category = reader.GetString(2),
                            FundingSource = reader.GetString(3),
                            SubmissionDate = reader.GetDateTime(4),
                            AwardDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                            Amount = reader.GetDecimal(6),
                            Status = reader.GetString(7),
                            LeadFacultyName = reader.GetString(8)
                        });
                    }
                }
            }
        }

        private void LoadProjects()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT p.ProjectID, p.Title, p.DueDate, 
                           u.FirstName + ' ' + u.LastName AS CreatedByName, 
                           b.Name AS BusinessPartnerName, 
                           g.GrantName
                    FROM Project p
                    JOIN [User] u ON p.CreatedBy = u.UserID
                    LEFT JOIN BusinessPartner b ON p.BusinessPartnerID = b.BusinessPartnerID
                    LEFT JOIN [Grant] g ON p.GrantID = g.GrantID", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Projects.Add(new Project
                        {
                            ProjectID = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            DueDate = reader.GetDateTime(2),
                            CreatedByName = reader.GetString(3),
                            BusinessPartnerName = reader.IsDBNull(4) ? "N/A" : reader.GetString(4),
                            GrantName = reader.IsDBNull(5) ? "N/A" : reader.GetString(5)
                        });
                    }
                }
            }
        }

        public class Grant
        {
            public int GrantID { get; set; }
            public string GrantName { get; set; } = "";
            public string Category { get; set; } = "";
            public string FundingSource { get; set; } = "";
            public DateTime SubmissionDate { get; set; }
            public DateTime? AwardDate { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; } = "";
            public string LeadFacultyName { get; set; } = "";
        }

        public class Project
        {
            public int ProjectID { get; set; }
            public string Title { get; set; } = "";
            public DateTime DueDate { get; set; }
            public string CreatedByName { get; set; } = "";
            public string? BusinessPartnerName { get; set; }
            public string? GrantName { get; set; }
        }
    }
}
