using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Lab1_Part3_Johnson_Imlay.Pages.DataClasses;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class AddMeetingMinutesModel : PageModel
    {
        [BindProperty]
        public MeetingMinute MeetingMinute { get; set; } = new();

        public List<DBClass.BusinessPartner> BusinessPartners { get; set; } = new();
        public List<DBClass.UserModel> Representatives { get; set; } = new();
        public List<DBClass.UserModel> Users { get; set; } = new();

        public void OnGet()
        {
            BusinessPartners = DBClass.LoadBusinessPartners();
            Representatives = DBClass.GetBusinessPartnerMessageSenders();
            Users = DBClass.GetBusinessPartnerMessageSenders();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                BusinessPartners = DBClass.LoadBusinessPartners();
                Representatives = DBClass.GetBusinessPartnerMessageSenders();
                Users = DBClass.GetBusinessPartnerMessageSenders();
                return Page();
            }

            bool success = DBClass.AddMeetingMinutes(
                MeetingMinute.BusinessPartnerID,
                MeetingMinute.RepresentativeID,
                MeetingMinute.MeetingWithID,
                MeetingMinute.MeetingDate,
                MeetingMinute.MinutesText
            );

            if (success)
                return RedirectToPage("/BusinessPartner/ViewMeetingMinutes");

            ModelState.AddModelError("", "Error adding meeting minutes.");
            return Page();
        }
    }
}
