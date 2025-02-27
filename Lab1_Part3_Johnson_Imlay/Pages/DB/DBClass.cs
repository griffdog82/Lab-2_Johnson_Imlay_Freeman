using System;
using System.Data;
using System.Data.SqlClient;


namespace Lab1_Part3_Johnson_Imlay.Pages.DB
{
    public class DBClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection Lab1DBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? Lab1DBConnString =
            "Server=Localhost;Database=Lab1;Trusted_Connection=True";

        public static int InsertUser(string username, string password, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                string query = @"INSERT INTO [User] 
                                 (Username, Password, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID) 
                                 VALUES (@Username, @Password, @Email, @FirstName, @LastName, @UserType, @Department, @AdminType, @BusinessPartnerID)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@UserType", userType);
                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
                    cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
                    cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public static List<BusinessPartner> GetBusinessPartners()
        {
            List<BusinessPartner> partners = new List<BusinessPartner>();
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
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
        public class BusinessPartner
        {
            public int BusinessPartnerID { get; set; }
            public string Name { get; set; } = "";
        }

         public static List<UserModel> GetFacultyMembers()
        {
            List<UserModel> facultyList = new List<UserModel>();

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = "SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'Faculty'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        facultyList.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }

            return facultyList;
        }

        // Retrieves projects for a specific faculty member or all projects if no facultyId is specified
        public static List<ProjectModel> GetProjectsByFacultyID(int? facultyId)
        {
            List<ProjectModel> projectList = new List<ProjectModel>();

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        P.ProjectID, 
                        P.Title, 
                        P.DueDate, 
                        U.FirstName + ' ' + U.LastName AS CreatedByName, 
                        BP.Name AS BusinessPartnerName, 
                        G.Category + ' (' + CAST(G.Amount AS NVARCHAR) + ')' AS GrantInfo
                    FROM Project P
                    INNER JOIN [User] U ON P.CreatedBy = U.UserID
                    LEFT JOIN BusinessPartner BP ON P.BusinessPartnerID = BP.BusinessPartnerID
                    LEFT JOIN [Grant] G ON P.GrantID = G.GrantID";

                if (facultyId.HasValue)
                {
                    query += " WHERE P.CreatedBy = @FacultyID";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (facultyId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@FacultyID", facultyId.Value);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projectList.Add(new ProjectModel
                            {
                                ProjectID = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                DueDate = reader.GetDateTime(2),
                                CreatedByName = reader.GetString(3),
                                BusinessPartnerName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                GrantInfo = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return projectList;
        }

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TaskID, Description, DueDate, Status FROM Task WHERE ProjectID = @ProjectID", conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", projectID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskModel
                            {
                                TaskID = reader.GetInt32(0),
                                Description = reader.GetString(1),
                                DueDate = reader.GetDateTime(2),
                                Status = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return tasks;
        }
        public static bool UpdateTaskStatus(int taskID, string newStatus)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = "UPDATE Task SET Status = @Status WHERE TaskID = @TaskID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        //ComposeMessage.cshtml.cs
        //public static List<UserModel> LoadUsers()
        //{
        //    List<UserModel> users = new();
        //    using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User]", conn))
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                users.Add(new UserModel
        //                {
        //                    UserID = reader.GetInt32(0),
        //                    FullName = reader.GetString(1)
        //                });
        //            }
        //        }
        //    }
        //    return users;
        //}
        public static (string Subject, string Body)? LoadReplyMessage(int messageID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT Subject, Body FROM Message WHERE MessageID = @MessageID", conn))
                {
                    cmd.Parameters.AddWithValue("@MessageID", messageID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ("RE: " + reader.GetString(0), "\n\n----- Original Message -----\n" + reader.GetString(1));
                        }
                    }
                }
            }
            return null;
        }
        public static bool SendMessage(int senderID, int recipientID, string subject, string body)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = "INSERT INTO Message (SenderID, RecipientID, Subject, Body, Timestamp) VALUES (@SenderID, @RecipientID, @Subject, @Body, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SenderID", senderID);
                    cmd.Parameters.AddWithValue("@RecipientID", recipientID);
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Body", body);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        //MessageList.cshtml.cs
        public static List<MessageModel> GetMessages(int recipientID, int? senderID = null)
        {
            List<MessageModel> messages = new();

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();

                string query = @"
            SELECT m.MessageID, 
                   u.FirstName + ' ' + u.LastName AS SenderName, 
                   m.Subject, 
                   m.Timestamp
            FROM Message m
            JOIN [User] u ON m.SenderID = u.UserID
            WHERE m.RecipientID = @RecipientID";

                if (senderID.HasValue)
                {
                    query += " AND m.SenderID = @SenderID";
                }

                query += " ORDER BY m.Timestamp DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RecipientID", recipientID);

                    if (senderID.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@SenderID", senderID.Value);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new MessageModel
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

            return messages;
        }
        public class MessageModel
        {
            public int MessageID { get; set; }
            public string SenderName { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Body { get; set; } = "";
            public DateTime Timestamp { get; set; }
        }
        public static List<UserModel> GetMessageSenders()
        {
            List<UserModel> senders = new();

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT DISTINCT u.UserID, u.FirstName, u.LastName
FROM [User] u
JOIN Message m ON u.UserID = m.SenderID", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        senders.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2)
                        });
                    }
                }
            }

            return senders;
        }
        public static MessageModel? GetMessageByID(int messageID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT m.MessageID, u.FirstName + ' ' + u.LastName AS SenderName, 
                   m.Subject, m.Body, m.Timestamp
            FROM Message m
            JOIN [User] u ON m.SenderID = u.UserID
            WHERE m.MessageID = @MessageID", conn))
                {
                    cmd.Parameters.AddWithValue("@MessageID", messageID);

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

        public static int InsertGrantApplication(string category, string grantName, string fundingSource, DateTime submissionDate,
            DateTime? awardDate, decimal amount, string leadFacultyID, string? businessPartnerID, string status)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();

                string query = @"INSERT INTO [Grant] 
                               (Category, GrantName, FundingSource, SubmissionDate, AwardDate, Amount, LeadFacultyID, BusinessPartnerID, Status)
                               VALUES (@Category, @GrantName, @FundingSource, @SubmissionDate, @AwardDate, @Amount, @LeadFacultyID, @BusinessPartnerID, @Status)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@GrantName", grantName);
                    cmd.Parameters.AddWithValue("@FundingSource", fundingSource);
                    cmd.Parameters.AddWithValue("@SubmissionDate", submissionDate);
                    cmd.Parameters.AddWithValue("@AwardDate", awardDate.HasValue ? (object)awardDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@LeadFacultyID", leadFacultyID);
                    cmd.Parameters.AddWithValue("@BusinessPartnerID", string.IsNullOrWhiteSpace(businessPartnerID) ? DBNull.Value : businessPartnerID);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery();
                }
            }
        }






































































































































































































        #endregion //Should be at line 1001. End of NicoleZone region.

        // ================================
        // END: Nicole Section
        // ================================

        // ================================
        // BEGIN: Zach Section (BusinessPartner)
        // ================================
        #region ZachSection
        public static List<BusinessPartner> LoadBusinessPartners()
        {
            List<BusinessPartner> partners = new();
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
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

        public class BusinessPartner
        {
            public int BusinessPartnerID { get; set; }
            public string Name { get; set; } = "";
        }




































































































































































        // ================================
        // END: Zach Section
        // ================================
        #endregion // Should end at 1205.
        // ================================
        // BEGIN: Common Section (Model Definitions & Connection Methods)
        // ================================

        public class UserModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
        }

        public class ProjectModel
        {
            public int ProjectID { get; set; }
            public string Title { get; set; } = "";
            public DateTime DueDate { get; set; }
            public string CreatedByName { get; set; } = "";
            public string? BusinessPartnerName { get; set; }
            public string? GrantInfo { get; set; }
        }

        public static SqlDataReader UserReader()
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = Lab1DBConnection;
            cmdProductRead.Connection.ConnectionString = Lab1DBConnString;
            cmdProductRead.CommandText = "SELECT * FROM [User]";
            cmdProductRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }
        public static SqlDataReader BusinessPartnerReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM BusinessPartner", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all Grants
        public static SqlDataReader GrantReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Grant]", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all GrantTeam records
        public static SqlDataReader GrantTeamReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM GrantTeam", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all Projects
        public static SqlDataReader ProjectReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Project", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all ProjectAssignments
        public static SqlDataReader ProjectAssignmentReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProjectAssignment", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all Tasks
        public static SqlDataReader TaskReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Task", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all MeetingMinutes
        public static SqlDataReader MeetingMinuteReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MeetingMinute", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // Retrieve all Messages
        public static SqlDataReader MessageReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Message", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

    }
}