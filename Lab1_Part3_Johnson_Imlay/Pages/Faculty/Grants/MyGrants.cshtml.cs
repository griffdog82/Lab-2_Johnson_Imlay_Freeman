//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;

//namespace Lab1_Part3_Johnson_Imlay.Pages.Faculty.Grants
//{
//    public class MyGrantsModel : PageModel
//    {
//        public List<Grant> Grants { get; set; }

//        public void OnGet()
//        {
//            int FacultyId = GetFacultyId();  // Dynamically fetch FacultyId

//            Grants = GetGrantsByFaculty(FacultyId);
//        }

//        private int GetFacultyId()
//        {
//            // Logic to retrieve FacultyId for the logged-in user (this is a placeholder)
//            return 123; // Replace with actual logic to fetch FacultyId for the logged-in user
//        }

//        private List<Grant> GetGrantsByFaculty(int facultyId)
//        {
//            var grants = new List<Grant>();

//            using (SqlConnection conn = new SqlConnection("Server=localhost;Database=Lab1;Trusted_Connection=True;"))
//            {
//                conn.Open();

//                string query = @"
//                    SELECT GrantID, GrantName, Status
//                    FROM [Grant]
//                    WHERE @FacultyID IN (LeadFacultyID, BusinessPartnerID)
//                    ORDER BY SubmissionDate DESC";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            var grant = new Grant
//                            {
//                                GrantID = reader.GetInt32(0),
//                                GrantName = reader.GetString(1),
//                                Status = reader.GetString(2)
//                            };
//                            grants.Add(grant);
//                        }
//                    }
//                }
//            }

//            return grants;
//        }
//    }

//    // Grant model to hold the data retrieved from the database
//    public class Grant
//    {
//        public int GrantID { get; set; }
//        public string GrantName { get; set; }
//        public string Status { get; set; }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Faculty.Grants
{
    public class MyGrantsModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty(SupportsGet = true)]
        public int? FacultyID { get; set; } // Selected Faculty ID from dropdown

        public List<Grant> Grants { get; set; } = new();
        public List<SelectListItem> FacultyList { get; set; } = new();

        public void OnGet()
        {
            LoadFacultyList(); // Populate the dropdown

            if (FacultyID.HasValue)
            {
                Grants = GetGrantsByFaculty(FacultyID.Value);
            }
        }

        private void LoadFacultyList()
        {
            FacultyList = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'Faculty'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FacultyList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private List<Grant> GetGrantsByFaculty(int facultyId)
        {
            var grants = new List<Grant>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT GrantID, GrantName, Status
                    FROM [Grant]
                    WHERE @FacultyID IN (LeadFacultyID, BusinessPartnerID)
                    ORDER BY SubmissionDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            grants.Add(new Grant
                            {
                                GrantID = reader.GetInt32(0),
                                GrantName = reader.GetString(1),
                                Status = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return grants;
        }
    }

    // Grant model to hold the data retrieved from the database
    public class Grant
    {
        public int GrantID { get; set; }
        public string GrantName { get; set; }
        public string Status { get; set; }
    }
}

