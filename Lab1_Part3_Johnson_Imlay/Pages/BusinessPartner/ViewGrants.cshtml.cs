using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ViewGrantsModel : PageModel
    {
        public List<Grant> Grants { get; set; }

        public void OnGet()
        {
            string connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";
            Grants = new List<Grant>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT GrantID, GrantName, Category, FundingSource, SubmissionDate, AwardDate, Amount, Status FROM [Grant]"; // Add brackets around Grant
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var grant = new Grant
                        {
                            GrantID = reader.GetInt32(0),
                            GrantName = reader.GetString(1),
                            Category = reader.GetString(2),
                            FundingSource = reader.GetString(3),
                            SubmissionDate = reader.GetDateTime(4),
                            AwardDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            Amount = reader.GetDecimal(6),
                            Status = reader.GetString(7)
                        };
                        Grants.Add(grant);
                    }
                }
            }
        }
    }

    public class Grant
    {
        public int GrantID { get; set; }
        public string GrantName { get; set; }
        public string Category { get; set; }
        public string FundingSource { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? AwardDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}