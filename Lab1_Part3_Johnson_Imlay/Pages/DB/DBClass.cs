//using System;
//using System.Data;
//using System.Data.SqlClient;


//namespace Lab1_Part3_Johnson_Imlay.Pages.DB
//{
//    public class DBClass
//    {
//        // Use this class to define methods that make connecting to
//        // and retrieving data from the DB easier.

//        // Connection Object at Data Field Level
//        public static SqlConnection Lab1DBConnection = new SqlConnection();

//        // Connection String - How to find and connect to DB
//        private static readonly String? Lab1DBConnString =
//            "Server=Localhost;Database=Lab1;Trusted_Connection=True";




//        public static bool AddUser(string username, string password, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
//        {
//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                string query = @"INSERT INTO [User] 
//                        (Username, Password, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID) 
//                        VALUES (@Username, @Password, @Email, @FirstName, @LastName, @UserType, @Department, @AdminType, @BusinessPartnerID)";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@Username", username);
//                    cmd.Parameters.AddWithValue("@Password", password);  // Should be hashed
//                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
//                    cmd.Parameters.AddWithValue("@FirstName", firstName);
//                    cmd.Parameters.AddWithValue("@LastName", lastName);
//                    cmd.Parameters.AddWithValue("@UserType", userType);
//                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
//                    cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
//                    cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

//                    return cmd.ExecuteNonQuery() > 0;
//                }
//            }
//        }
//        public static bool EditUser(int userID, string username, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
//        {
//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                string query = @"UPDATE [User] 
//                        SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, 
//                            UserType = @UserType, Department = @Department, AdminType = @AdminType, BusinessPartnerID = @BusinessPartnerID
//                        WHERE UserID = @UserID";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userID);
//                    cmd.Parameters.AddWithValue("@Username", username);
//                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
//                    cmd.Parameters.AddWithValue("@FirstName", firstName);
//                    cmd.Parameters.AddWithValue("@LastName", lastName);
//                    cmd.Parameters.AddWithValue("@UserType", userType);
//                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
//                    cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
//                    cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

//                    return cmd.ExecuteNonQuery() > 0;
//                }
//            }
//        }

//        // This method should allow Ezell's comments to be rectified and the method to be used in the AddUser.cshtml.cs file
//        public static List<UserModel> LoadUsers()
//        {
//            List<UserModel> users = new();

//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                using (SqlCommand cmd = new SqlCommand(@"
//            SELECT u.UserID, u.Username, u.Email, u.FirstName, u.LastName, u.UserType, 
//                   u.Department, u.AdminType, b.Name AS BusinessPartnerName
//            FROM [User] u
//            LEFT JOIN BusinessPartner b ON u.BusinessPartnerID = b.BusinessPartnerID", conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        users.Add(new UserModel
//                        {
//                            UserID = reader.GetInt32(0),
//                            Username = reader.GetString(1),
//                            Email = reader.IsDBNull(2) ? "N/A" : reader.GetString(2),
//                            FirstName = reader.GetString(3),
//                            LastName = reader.GetString(4),
//                            UserType = reader.GetString(5),
//                            Department = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
//                            AdminType = reader.IsDBNull(7) ? "N/A" : reader.GetString(7),
//                            BusinessPartnerID = reader.IsDBNull(8) ? "N/A" : reader.GetString(8)
//                        });
//                    }
//                }
//            }

//            return users;
//        } //This should replace the old method located in the ViewUsers.cshtml.cs file. This method is called in the OnGet method of the EditUser.cshtml.cs file.

//        public static string? LoadProjectTitle(int projectID)
//        {
//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Project WHERE ProjectID = @ProjectID", conn))
//                {
//                    cmd.Parameters.AddWithValue("@ProjectID", projectID);
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            return reader.GetString(0);
//                        }
//                    }
//                }
//            }
//            return null;
//        }

//        public static bool AddTask(int projectID, string description, DateTime dueDate, string status)
//        {
//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                string query = "INSERT INTO Task (ProjectID, Description, DueDate, Status) VALUES (@ProjectID, @Description, @DueDate, @Status)";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@ProjectID", projectID);
//                    cmd.Parameters.AddWithValue("@Description", description);
//                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
//                    cmd.Parameters.AddWithValue("@Status", status);

//                    return cmd.ExecuteNonQuery() > 0;
//                }
//            }
//        }




//        public static List<BusinessPartner> LoadBusinessPartners()
//        {
//            List<BusinessPartner> partners = new();
//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        partners.Add(new BusinessPartner
//                        {
//                            BusinessPartnerID = reader.GetInt32(0),
//                            Name = reader.GetString(1)
//                        });
//                    }
//                }
//            }
//            return partners;
//        }
//        public class BusinessPartner
//        {
//            public int BusinessPartnerID { get; set; }
//            public string Name { get; set; } = "";
//        }

//        public static List<UserModel> GetFacultyMembers()
//        {
//            List<UserModel> facultyList = new List<UserModel>();

//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();
//                string query = @"
//            SELECT UserID, Username, Email, FirstName, LastName, UserType, 
//                   Department, AdminType, BusinessPartnerID
//            FROM [User] 
//            WHERE UserType = 'Faculty'";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        facultyList.Add(new UserModel
//                        {
//                            UserID = reader.GetInt32(0),
//                            Username = reader.GetString(1),
//                            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
//                            FirstName = reader.GetString(3),
//                            LastName = reader.GetString(4),
//                            UserType = reader.GetString(5),
//                            Department = reader.IsDBNull(6) ? null : reader.GetString(6),
//                            AdminType = reader.IsDBNull(7) ? null : reader.GetString(7),
//                            // TODO BusinessPartnerID = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8) @Nicole -> Fix Me!
//                        });
//                    }
//                }
//            }

//            return facultyList;
//        }


//        // Retrieves projects for a specific faculty member or all projects if no facultyId is specified
//        public static List<ProjectModel> GetProjectsByFacultyID(int? facultyId)
//        {
//            List<ProjectModel> projectList = new List<ProjectModel>();

//            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
//            {
//                conn.Open();

//                string query = @"
//                    SELECT 
//                        P.ProjectID, 
//                        P.Title, 
//                        P.DueDate, 
//                        U.FirstName + ' ' + U.LastName AS CreatedByName, 
//                        BP.Name AS BusinessPartnerName, 
//                        G.Category + ' (' + CAST(G.Amount AS NVARCHAR) + ')' AS GrantInfo
//                    FROM Project P
//                    INNER JOIN [User] U ON P.CreatedBy = U.UserID
//                    LEFT JOIN BusinessPartner BP ON P.BusinessPartnerID = BP.BusinessPartnerID
//                    LEFT JOIN [Grant] G ON P.GrantID = G.GrantID";

//                if (facultyId.HasValue)
//                {
//                    query += " WHERE P.CreatedBy = @FacultyID";
//                }

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    if (facultyId.HasValue)
//                    {
//                        cmd.Parameters.AddWithValue("@FacultyID", facultyId.Value);
//                    }

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            projectList.Add(new ProjectModel
//                            {
//                                ProjectID = reader.GetInt32(0),
//                                Title = reader.GetString(1),
//                                DueDate = reader.GetDateTime(2),
//                                CreatedByName = reader.GetString(3),
//                                BusinessPartnerName = reader.IsDBNull(4) ? null : reader.GetString(4),
//                                GrantInfo = reader.IsDBNull(5) ? null : reader.GetString(5)
//                            });
//                        }
//                    }
//                }
//            }

//            return projectList;
//        }

//        public class UserModel
//        {
//            public int UserID { get; set; }
//            public string Username { get; set; } = "";
//            public string Email { get; set; } = "";
//            public string FirstName { get; set; } = "";
//            public string LastName { get; set; } = "";
//            public string UserType { get; set; } = "";
//            public string? Department { get; set; }
//            public string? AdminType { get; set; }
//            public string? BusinessPartnerID { get; set; }
//        } //This object replaces the old one in the ViewUsers.cshtml.cs file. This object is used in the LoadUsers method.


//        public class ProjectModel
//        {
//            public int ProjectID { get; set; }
//            public string Title { get; set; } = "";
//            public DateTime DueDate { get; set; }
//            public string CreatedByName { get; set; } = "";
//            public string? BusinessPartnerName { get; set; }
//            public string? GrantInfo { get; set; }
//        }

//        //Connection Methodsb ---   These methods are used to connect to the database and retrieve data from the database.

//        //Basic Product Reader

//        public static SqlDataReader UserReader()
//        {
//            SqlCommand cmdProductRead = new SqlCommand();
//            cmdProductRead.Connection = Lab1DBConnection;
//            cmdProductRead.Connection.ConnectionString = Lab1DBConnString;
//            cmdProductRead.CommandText = "SELECT * FROM [User]";
//            cmdProductRead.Connection.Open(); // Open connection here, close in Model!

//            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

//            return tempReader;
//        }
//        public static SqlDataReader BusinessPartnerReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM BusinessPartner", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all Grants
//        public static SqlDataReader GrantReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM [Grant]", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all GrantTeam records
//        public static SqlDataReader GrantTeamReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM GrantTeam", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all Projects
//        public static SqlDataReader ProjectReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Project", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all ProjectAssignments
//        public static SqlDataReader ProjectAssignmentReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM ProjectAssignment", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all Tasks
//        public static SqlDataReader TaskReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Task", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all MeetingMinutes
//        public static SqlDataReader MeetingMinuteReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM MeetingMinute", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//        // Retrieve all Messages
//        public static SqlDataReader MessageReader()
//        {
//            SqlCommand cmd = new SqlCommand("SELECT * FROM Message", Lab1DBConnection);
//            cmd.Connection.Open();
//            SqlDataReader tempReader = cmd.ExecuteReader();
//            return tempReader;
//        }

//    }
//}

using System;
using System.Data;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.DB
{
    public class DBClass
    {
        public static SqlConnection Lab1DBConnection = new SqlConnection();
        private static readonly string? Lab1DBConnString = "Server=Localhost;Database=Lab1;Trusted_Connection=True";

        // ================================
        // BEGIN: Griffin Section
        // ================================
        #region GriffinLand
        public static bool AddUser(string username, string password, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = @"INSERT INTO [User] 
                        (Username, Password, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID) 
                        VALUES (@Username, @Password, @Email, @FirstName, @LastName, @UserType, @Department, @AdminType, @BusinessPartnerID)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);  // Should be hashed
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@UserType", userType);
                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
                    cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
                    cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool EditUser(int userID, string username, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = @"UPDATE [User] 
                        SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, 
                            UserType = @UserType, Department = @Department, AdminType = @AdminType, BusinessPartnerID = @BusinessPartnerID
                        WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@UserType", userType);
                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
                    cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
                    cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<UserModel> LoadUsers()
        {
            List<UserModel> users = new();

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT u.UserID, u.Username, u.Email, u.FirstName, u.LastName, u.UserType, 
                   u.Department, u.AdminType, b.Name AS BusinessPartnerName
            FROM [User] u
            LEFT JOIN BusinessPartner b ON u.BusinessPartnerID = b.BusinessPartnerID", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.IsDBNull(2) ? "N/A" : reader.GetString(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            UserType = reader.GetString(5),
                            Department = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
                            AdminType = reader.IsDBNull(7) ? "N/A" : reader.GetString(7),
                            BusinessPartnerID = reader.IsDBNull(8) ? "N/A" : reader.GetString(8)
                        });
                    }
                }
            }

            return users;
        }

        public static string? LoadProjectTitle(int projectID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Project WHERE ProjectID = @ProjectID", conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", projectID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return null;
        }

        public static bool AddTask(int projectID, string description, DateTime dueDate, string status)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = "INSERT INTO Task (ProjectID, Description, DueDate, Status) VALUES (@ProjectID, @Description, @DueDate, @Status)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", projectID);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public static bool EditTask(int taskID, string description, DateTime dueDate, string status)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = "UPDATE Task SET Description = @Description, DueDate = @DueDate, Status = @Status WHERE TaskID = @TaskID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public class TaskModel
        {
            public int TaskID { get; set; }
            public int ProjectID { get; set; }
            public string Description { get; set; } = "";
            public DateTime DueDate { get; set; }
            public string Status { get; set; } = "";
        }
        public static TaskModel? GetTask(int taskID)
        {
            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProjectID, Description, DueDate, Status FROM Task WHERE TaskID = @TaskID", conn))
                {
                    cmd.Parameters.AddWithValue("@TaskID", taskID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TaskModel
                            {
                                TaskID = taskID,
                                ProjectID = reader.GetInt32(0),
                                Description = reader.GetString(1),
                                DueDate = reader.GetDateTime(2),
                                Status = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }



















































































































































        #endregion // SHould be at line 701.  This is the end of the GriffinLand region.
        // ================================
        // END: Griffin Section
        // ================================

        // ================================
        // BEGIN: Nicole Section (Faculty Classes)
        // ================================
        #region NicoleZone


        public static List<UserModel> GetFacultyMembers()
        {
            List<UserModel> facultyList = new List<UserModel>();

            using (SqlConnection conn = new SqlConnection(Lab1DBConnString))
            {
                conn.Open();
                string query = @"
            SELECT UserID, Username, Email, FirstName, LastName, UserType, 
                   Department, AdminType, BusinessPartnerID
            FROM [User] 
            WHERE UserType = 'Faculty'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        facultyList.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            UserType = reader.GetString(5),
                            Department = reader.IsDBNull(6) ? null : reader.GetString(6),
                            AdminType = reader.IsDBNull(7) ? null : reader.GetString(7)
                            // TODO: BusinessPartnerID assignment @Nicole -> Fix Me!
                        });
                    }
                }
            }

            return facultyList;
        }

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
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string UserType { get; set; } = "";
            public string? Department { get; set; }
            public string? AdminType { get; set; }
            public string? BusinessPartnerID { get; set; }
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

        public static SqlDataReader GrantReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Grant]", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader GrantTeamReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM GrantTeam", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader ProjectReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Project", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader ProjectAssignmentReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProjectAssignment", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader TaskReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Task", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader MeetingMinuteReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MeetingMinute", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader MessageReader()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Message", Lab1DBConnection);
            cmd.Connection.Open();
            SqlDataReader tempReader = cmd.ExecuteReader();
            return tempReader;
        }

        // ================================
        // END: Common Section
        // ================================
    }
}
