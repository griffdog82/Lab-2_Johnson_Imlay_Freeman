using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ViewMessagesModel : PageModel
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
            Senders = DBClass.GetBusinessPartnerMessageSenders();
            Messages = DBClass.GetBusinessPartnerMessages(UserID, senderId); // Load messages for the user

            if (id.HasValue)
            {
                SelectedMessage = DBClass.GetBusinessPartnerMessageByID(id.Value);
            }
        }
    }
}

