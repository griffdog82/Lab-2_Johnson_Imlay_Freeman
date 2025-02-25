using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Projects
{
    public class ProjectTaskManagementModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int ProjectID { get; set; }

        public string ProjectTitle { get; set; } = "";
        public List<TaskModel> Tasks { get; set; } = new();

        public void OnGet(int id)
        {
            ProjectID = id;  // Ensure ProjectID is set dynamically
            LoadProjectDetails();
            LoadTasks();
        }


        public IActionResult OnPostUpdateStatus(int TaskID, string NewStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Task SET Status = @Status WHERE TaskID = @TaskID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskID", TaskID);
                        cmd.Parameters.AddWithValue("@Status", NewStatus);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating task status: " + ex.Message);
            }

            LoadProjectDetails();
            LoadTasks();
            return Page();
        }

        private void LoadProjectDetails()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Project WHERE ProjectID = @ProjectID", conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProjectTitle = reader.GetString(0);
                        }
                    }
                }
            }
        }

        private void LoadTasks()
        {
            Tasks.Clear(); // Prevent old data from persisting

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TaskID, Description, DueDate, Status FROM Task WHERE ProjectID = @ProjectID", conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tasks.Add(new TaskModel
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
        }



        public class TaskModel
        {
            public int TaskID { get; set; }
            public string Description { get; set; } = "";
            public DateTime DueDate { get; set; }
            public string Status { get; set; } = "";
        }
    }
}
