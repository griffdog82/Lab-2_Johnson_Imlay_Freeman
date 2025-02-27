using Lab1_Part3_Johnson_Imlay.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using static Lab1_Part3_Johnson_Imlay.Pages.DB.DBClass;

namespace Lab1_Part3_Johnson_Imlay.Pages.Faculty.Grants
{
    public class GrantDetailsModel : PageModel
    {
        public GrantModel? GrantDetails { get; set; }

        public void OnGet(int grantId)
        {
            GrantDetails = DBClass.GetGrantDetails(grantId);
        }

    }
}

