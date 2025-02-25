using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Tasks
{
    public class AddTaskModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int ProjectID { get; set; }

        public string ProjectTitle { get; set; } = "";

        [BindProperty, Required]
        public string Description { get; set; } = "";

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required]
        public string Status { get; set; } = "Pending";

        public string Message { get; set; } = "";

        public void OnGet(int projectId)
        {
            ProjectID = projectId;
            LoadProjectTitle();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadProjectTitle();
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Task (ProjectID, Description, DueDate, Status) VALUES (@ProjectID, @Description, @DueDate, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DueDate", DueDate);
                        cmd.Parameters.AddWithValue("@Status", Status);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Message = "Task added successfully!";
                            return RedirectToPage("/Admin/Projects/ProjectTaskManagement", new { id = ProjectID });
                        }
                        else
                        {
                            Message = "Error adding task.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            LoadProjectTitle();
            return Page();
        }

        private void LoadProjectTitle()
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
    }
}
