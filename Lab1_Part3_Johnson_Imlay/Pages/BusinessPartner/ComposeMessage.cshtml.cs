using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ComposeMessageModel : PageModel
    {
        [BindProperty]
        public int RecipientID { get; set; }

        [BindProperty]
        public string Subject { get; set; } = "";

        [BindProperty]
        public string Body { get; set; } = "";

        [BindProperty]
        public int UserID { get; set; } // Replace with actual logged-in user ID

        public List<DBClass.UserModel> Users { get; set; } = new();
        public string Message { get; set; } = "";

        public void OnGet(int? replyTo)
        {
            Users = DBClass.LoadUsers();

            if (replyTo.HasValue)
            {
                var replyMessage = DBClass.LoadBusinessPartnerReplyMessage(replyTo.Value);
                if (replyMessage.HasValue)
                {
                    Subject = replyMessage.Value.Subject;
                    Body = replyMessage.Value.Body;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Users = DBClass.LoadUsers();
                return Page();
            }

            if (DBClass.SendBusinessPartnerMessage(UserID, RecipientID, Subject, Body))
            {
                Message = "Message sent successfully!";
                return RedirectToPage("ViewMessages");
            }
            else
            {
                Message = "Error sending message.";
            }

            Users = DBClass.LoadUsers();
            return Page();
        }
    }
}