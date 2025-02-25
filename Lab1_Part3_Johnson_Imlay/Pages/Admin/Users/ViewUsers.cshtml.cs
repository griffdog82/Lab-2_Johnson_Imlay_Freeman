using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using Lab1_Part3_Johnson_Imlay.Pages.DB;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Users
{
    public class ViewUsersModel : PageModel
    {

        public List<DBClass.UserModel> Users { get; set; } = new();

        public void OnGet()
        {
            Users = DBClass.LoadUsers();
        }

        

       
    }
}
