using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Lab1_Part3_Johnson_Imlay.Pages.DB;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Messages
{
    public class MessageListModel : PageModel
    {
    

        [BindProperty]
        public int MessageID { get; set; }

        [BindProperty]
        public int UserID { get; set; } // Replace with actual logged-in user ID

        [BindProperty]
        public List<DBClass.MessageModel> Messages { get; set; } = new();

        [BindProperty]
        public DBClass.MessageModel? SelectedMessage { get; set; } // Holds a single message when viewing

        public List<DBClass.UserModel> Senders { get; set; } = new();
        public int? SelectedSenderID { get; set; }

        public void OnGet(int? id, int? senderId)
        {
            SelectedSenderID = senderId;
            Senders = DBClass.GetMessageSenders();
            Messages = DBClass.GetMessages(UserID, senderId); // Load messages for the user

            if (id.HasValue)
            {
                SelectedMessage = DBClass.GetMessageByID(id.Value);
            }
        }




        

       
    }
}
