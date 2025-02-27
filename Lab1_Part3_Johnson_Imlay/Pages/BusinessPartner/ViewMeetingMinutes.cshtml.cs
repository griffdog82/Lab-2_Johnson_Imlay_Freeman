using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Lab1_Part3_Johnson_Imlay.Pages.DataClasses;

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ViewMeetingMinutesModel : PageModel
    {
        public List<MeetingMinute> MeetingMinutes { get; set; } = new();
        public MeetingMinute? SelectedMeetingMinute { get; set; }

        public void OnGet(int? minuteID)
        {
            MeetingMinutes = DBClass.GetBusinessPartnerMeetingMinutes();

            if (minuteID.HasValue)
            {
                SelectedMeetingMinute = DBClass.GetBusinessPartnerMeetingMinuteByID(minuteID.Value);
            }
        }
    }
}
