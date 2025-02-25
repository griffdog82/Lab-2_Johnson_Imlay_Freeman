using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Faculty.Grants
{
    public class GrantDetailsModel : PageModel
    {
        public int GrantID { get; set; }
        public string GrantName { get; set; }
        public string Category { get; set; }
        public string FundingSource { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime AwardDate { get; set; }
        public decimal Amount { get; set; }
        public int LeadFacultyID { get; set; }
        public string BusinessPartnerID { get; set; }
        public string Status { get; set; }

        public void OnGet(int grantId)
        {
            GrantID = grantId;
            GetGrantDetails(grantId);
        }

        private void GetGrantDetails(int grantId)
        {
            using (SqlConnection conn = new SqlConnection("Server=localhost;Database=Lab1;Trusted_Connection=True;"))
            {
                conn.Open();

                string query = @"
                    SELECT GrantName, Category, FundingSource, SubmissionDate, AwardDate, Amount, LeadFacultyID, BusinessPartnerID, Status
                    FROM [Grant]
                    WHERE GrantID = @GrantID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GrantID", grantId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            GrantName = reader.GetString(0);
                            Category = reader.GetString(1);
                            FundingSource = reader.GetString(2);
                            SubmissionDate = reader.GetDateTime(3);
                            AwardDate = reader.GetDateTime(4);
                            Amount = reader.GetDecimal(5);
                            LeadFacultyID = reader.GetInt32(6);
                            BusinessPartnerID = reader.GetString(7);
                            Status = reader.GetString(8);
                        }
                    }
                }
            }
        }
    }
}

