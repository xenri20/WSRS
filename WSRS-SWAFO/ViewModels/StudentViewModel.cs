using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WSRS_SWAFO.ViewModels
{
    public class StudentViewModel
    {
        public StudentRecordModel NewStudent { get; set; }
        [ValidateNever]
        public IQueryable<StudentRecordModel> ExistingStudents { get; set; }
    }
}
