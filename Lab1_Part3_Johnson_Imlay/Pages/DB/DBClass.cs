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
        // Commit 2 
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

        // Define the UserModel and ProjectModel within DBClass for use
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

        //Connection Methods:

        //Basic Product Reader

        //Leave alone for now, no need to overhaul. Maybe on Capstone
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