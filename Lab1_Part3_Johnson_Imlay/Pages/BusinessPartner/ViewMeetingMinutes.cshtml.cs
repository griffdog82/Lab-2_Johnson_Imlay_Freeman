using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ViewMeetingMinutesModel : PageModel
    {
        public List<MeetingMinute> MeetingMinutes { get; set; }

        public void OnGet()
        {
            string connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";
            MeetingMinutes = new List<MeetingMinute>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MinuteID, BusinessPartnerID, RepresentativeID, MeetingWithID, MeetingDate, MinutesText FROM MeetingMinute";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var meetingMinute = new MeetingMinute
                        {
                            MinuteID = reader.GetInt32(0),
                            BusinessPartnerID = reader.GetInt32(1),
                            RepresentativeID = reader.GetInt32(2),
                            MeetingWithID = reader.GetInt32(3),
                            MeetingDate = reader.GetDateTime(4),
                            MinutesText = reader.GetString(5)
                        };
                        MeetingMinutes.Add(meetingMinute);
                    }
                }
            }
        }
    }

    public class MeetingMinute
    {
        public int MinuteID { get; set; }
        public int BusinessPartnerID { get; set; }
        public int RepresentativeID { get; set; }
        public int MeetingWithID { get; set; }
        public DateTime MeetingDate { get; set; }
        public string MinutesText { get; set; }
    }
}
