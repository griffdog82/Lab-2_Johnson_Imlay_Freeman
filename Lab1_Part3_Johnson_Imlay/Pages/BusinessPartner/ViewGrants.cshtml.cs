using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Lab1_Part3_Johnson_Imlay.Pages.DataClasses; // Ensure this is here

namespace Lab1_Part3_Johnson_Imlay.Pages.BusinessPartner
{
    public class ViewGrantsModel : PageModel
    {
        public List<Grant> Grants { get; set; } = new();
        public Grant? SelectedGrant { get; set; }

        public void OnGet(int? grantID)
        {
            Grants = DBClass.GetBusinessPartnerGrants();

            if (grantID.HasValue)
            {
                SelectedGrant = DBClass.GetBusinessPartnerGrantByID(grantID.Value);
            }
        }
    }
}
