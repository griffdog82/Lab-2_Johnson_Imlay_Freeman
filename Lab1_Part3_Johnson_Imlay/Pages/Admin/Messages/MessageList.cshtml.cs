using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Messages
{
    public class MessageListModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int MessageID { get; set; }

        [BindProperty]
        public int UserID { get; set; } // Replace with actual logged-in user ID

        [BindProperty]
        public List<MessageModel> Messages { get; set; } = new();

        [BindProperty]
        public MessageModel? SelectedMessage { get; set; } // Holds a single message when viewing

        public List<UserModel> Senders { get; set; } = new();
        public int? SelectedSenderID { get; set; }

        public void OnGet(int? id, int? senderId)
        {
            SelectedSenderID = senderId;
            LoadSenders();
            LoadMessages(senderId);

            if (id.HasValue)
            {
                SelectedMessage = GetMessageByID(id.Value);
            }
        }

        private void LoadMessages(int? senderId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT m.MessageID, 
                           u.FirstName + ' ' + u.LastName AS SenderName, 
                           m.Subject, 
                           m.Timestamp
                    FROM Message m
                    JOIN [User] u ON m.SenderID = u.UserID
                    WHERE m.RecipientID = @UserID";

                if (senderId.HasValue)
                {
                    query += " AND m.SenderID = @SenderID";
                }

                query += " ORDER BY m.Timestamp DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", 1); // Replace with logged-in user ID later

                    if (senderId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@SenderID", senderId.Value);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Messages.Add(new MessageModel
                            {
                                MessageID = reader.GetInt32(0),
                                SenderName = reader.GetString(1),
                                Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                                Timestamp = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }
        }

        private void LoadSenders()
        {
            Senders = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT u.UserID, u.FirstName + ' ' + u.LastName AS FullName 
                    FROM [User] u
                    JOIN Message m ON u.UserID = m.SenderID", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Senders.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private MessageModel? GetMessageByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT m.MessageID, u.FirstName + ' ' + u.LastName AS SenderName, 
                           m.Subject, m.Body, m.Timestamp
                    FROM Message m
                    JOIN [User] u ON m.SenderID = u.UserID
                    WHERE m.MessageID = @MessageID", conn))
                {
                    cmd.Parameters.AddWithValue("@MessageID", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MessageModel
                            {
                                MessageID = reader.GetInt32(0),
                                SenderName = reader.GetString(1),
                                Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                                Body = reader.GetString(3),
                                Timestamp = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public class MessageModel
        {
            public int MessageID { get; set; }
            public string SenderName { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Body { get; set; } = "";
            public DateTime Timestamp { get; set; }
        }

        public class UserModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
        }
    }
}
