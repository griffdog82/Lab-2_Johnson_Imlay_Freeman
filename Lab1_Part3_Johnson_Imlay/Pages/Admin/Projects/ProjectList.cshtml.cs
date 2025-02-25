using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Lab1_Part3_Johnson_Imlay.Pages.DB;

namespace Lab1_Part3_Johnson_Imlay.Pages.Admin.Projects
{
    public class ProjectListModel : PageModel
    {
        public List<DBClass.ProjectModel> Projects { get; set; } = new();
        public List<DBClass.UserModel> FacultyMembers { get; set; } = new();
        public int? SelectedFacultyID { get; set; }

        public void OnGet(int? facultyId)
        {
            SelectedFacultyID = facultyId;
            FacultyMembers = DBClass.GetFacultyMembers();
            Projects = DBClass.GetProjectsByFacultyID(facultyId);
        }
    }
}
