using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Messages
{
    public class ViewMessagesModel : PageModel
    {
        private readonly string _dbConnection = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int MsgID { get; set; }

        [BindProperty]
        public int CurrentUserID { get; set; } // Replace with actual logged-in user ID

        [BindProperty]
        public List<MessageItem> MsgList { get; set; } = new();

        [BindProperty]
        public MessageItem? OpenMessage { get; set; } // Holds a single message when viewing

        public List<UserItem> MsgSenders { get; set; } = new();
        public int? FilteredSenderID { get; set; }

        public void OnGet(int? messageId, int? senderFilter)
        {
            FilteredSenderID = senderFilter;
            FetchSenders();
            FetchMessages(senderFilter);

            if (messageId.HasValue)
            {
                OpenMessage = RetrieveMessageByID(messageId.Value);
            }
        }

        private void FetchMessages(int? senderFilter)
        {
            using (SqlConnection conn = new SqlConnection(_dbConnection))
            {
                conn.Open();

                string query = @"
                    SELECT m.MessageID, 
                           u.FirstName + ' ' + u.LastName AS SenderFullName, 
                           m.Subject, 
                           m.Timestamp
                    FROM Message m
                    JOIN [User] u ON m.SenderID = u.UserID
                    WHERE m.RecipientID = @CurrentUserID";

                if (senderFilter.HasValue)
                {
                    query += " AND m.SenderID = @SenderFilter";
                }

                query += " ORDER BY m.Timestamp DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CurrentUserID", 1); // Replace with logged-in user ID later

                    if (senderFilter.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@SenderFilter", senderFilter.Value);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MsgList.Add(new MessageItem
                            {
                                MsgID = reader.GetInt32(0),
                                SenderFullName = reader.GetString(1),
                                Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                                SentTime = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }
        }

        private void FetchSenders()
        {
            MsgSenders = new List<UserItem>();
            using (SqlConnection conn = new SqlConnection(_dbConnection))
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
                        MsgSenders.Add(new UserItem
                        {
                            UserAccountID = reader.GetInt32(0),
                            DisplayName = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private MessageItem? RetrieveMessageByID(int messageId)
        {
            using (SqlConnection conn = new SqlConnection(_dbConnection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT m.MessageID, u.FirstName + ' ' + u.LastName AS SenderFullName, 
                           m.Subject, m.Body, m.Timestamp
                    FROM Message m
                    JOIN [User] u ON m.SenderID = u.UserID
                    WHERE m.MessageID = @MsgID", conn))
                {
                    cmd.Parameters.AddWithValue("@MsgID", messageId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MessageItem
                            {
                                MsgID = reader.GetInt32(0),
                                SenderFullName = reader.GetString(1),
                                Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                                BodyContent = reader.GetString(3),
                                SentTime = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public class MessageItem
        {
            public int MsgID { get; set; }
            public string SenderFullName { get; set; } = "";
            public string Subject { get; set; } = "";
            public string BodyContent { get; set; } = "";
            public DateTime SentTime { get; set; }
        }

        public class UserItem
        {
            public int UserAccountID { get; set; }
            public string DisplayName { get; set; } = "";
        }
    }
}
