using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ComposeMessageModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int RecipientID { get; set; }

        [BindProperty]
        public string Subject { get; set; } = "";

        [BindProperty]
        public string Body { get; set; } = "";

        [BindProperty]
        public int UserID { get; set; } // Placeholder for logged-in user ID

        public List<UserModel> Users { get; set; } = new();

        public string ErrorMessage { get; set; } = "";

        public void OnGet()
        {
            LoadUsers();
        }

        public IActionResult OnPost()
        {
            if (RecipientID == 0 || string.IsNullOrEmpty(Subject) || string.IsNullOrEmpty(Body))
            {
                ErrorMessage = "⚠️ Please fill out all fields.";
                LoadUsers();
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Message (SenderID, RecipientID, Subject, Body, Timestamp) VALUES (@SenderID, @RecipientID, @Subject, @Body, @Timestamp)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SenderID", 1); // Replace with logged-in user ID
                        cmd.Parameters.AddWithValue("@RecipientID", RecipientID);
                        cmd.Parameters.AddWithValue("@Subject", Subject);
                        cmd.Parameters.AddWithValue("@Body", Body);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/BusinessPartner/ViewMessages");
            }
            catch (Exception ex)
            {
                ErrorMessage = "⚠️ Database Error: " + ex.Message;
            }

            LoadUsers();
            return Page();
        }

        private void LoadUsers()
        {
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

       

        public class UserModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
        }
    }
}
