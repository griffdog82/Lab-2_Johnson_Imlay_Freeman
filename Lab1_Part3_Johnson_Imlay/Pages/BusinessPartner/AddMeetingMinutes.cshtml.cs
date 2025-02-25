using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using Lab1_Part3_Johnson_Imlay.Pages.DataClasses;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class AddMeetingMinutesModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public MeetingMinute MeetingMinute { get; set; } = new();

        public List<BusinessPartner> BusinessPartners { get; set; } = new();
        public List<UserModel> Representatives { get; set; } = new();
        public List<UserModel> Users { get; set; } = new();

        public void OnGet()
        {
            LoadBusinessPartners();
            LoadRepresentatives();
            LoadUsers();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadBusinessPartners();
                LoadRepresentatives();
                LoadUsers();
                return Page();
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO MeetingMinute (BusinessPartnerID, RepresentativeID, MeetingWithID, MeetingDate, MinutesText) " +
                               "VALUES (@BusinessPartnerID, @RepresentativeID, @MeetingWithID, @MeetingDate, @MinutesText)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BusinessPartnerID", MeetingMinute.BusinessPartnerID);
                    command.Parameters.AddWithValue("@RepresentativeID", MeetingMinute.RepresentativeID);
                    command.Parameters.AddWithValue("@MeetingWithID", MeetingMinute.MeetingWithID);
                    command.Parameters.AddWithValue("@MeetingDate", MeetingMinute.MeetingDate);
                    command.Parameters.AddWithValue("@MinutesText", MeetingMinute.MinutesText);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/BusinessPartner/ViewMeetingMinutes");
        }

        private void LoadBusinessPartners()
        {
            BusinessPartners = new List<BusinessPartner>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BusinessPartners.Add(new BusinessPartner
                        {
                            BusinessPartnerID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private void LoadRepresentatives()
        {
            Representatives = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'RepOfBusiness'", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Representatives.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private void LoadUsers()
        {
            Users = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User]", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }
        }
        //public class MeetingMinute
        //{
        //    public int BusinessPartnerID { get; set; }
        //    public int RepresentativeID { get; set; }
        //    public int MeetingWithID { get; set; }
        //    public DateTime MeetingDate { get; set; }
        //    public string MinutesText { get; set; } = "";
        //}


        public class BusinessPartner
        {
            public int BusinessPartnerID { get; set; }
            public string Name { get; set; } = "";
        }

        public class UserModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
        }
    }
}
