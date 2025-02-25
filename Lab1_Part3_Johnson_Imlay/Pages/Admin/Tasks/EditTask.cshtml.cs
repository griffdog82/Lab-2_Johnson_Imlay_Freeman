using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Tasks
{
    public class EditTaskModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int TaskID { get; set; }

        [BindProperty]
        public int ProjectID { get; set; }

        [BindProperty, Required]
        public string Description { get; set; } = "";

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required]
        public string Status { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet(int id)
        {
            TaskID = id;
            LoadTask();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Task SET Description = @Description, DueDate = @DueDate, Status = @Status WHERE TaskID = @TaskID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskID", TaskID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DueDate", DueDate);
                        cmd.Parameters.AddWithValue("@Status", Status);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Message = "Task updated successfully!";
                            return RedirectToPage("/Admin/Projects/ProjectTaskManagement", new { id = ProjectID });
                        }
                        else
                        {
                            Message = "Error updating task.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            return Page();
        }

        private void LoadTask()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProjectID, Description, DueDate, Status FROM Task WHERE TaskID = @TaskID", conn))
                {
                    cmd.Parameters.AddWithValue("@TaskID", TaskID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProjectID = reader.GetInt32(0);
                            Description = reader.GetString(1);
                            DueDate = reader.GetDateTime(2);
                            Status = reader.GetString(3);
                        }
                    }
                }
            }
        }
    }
}
