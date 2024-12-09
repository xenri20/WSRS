using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WSRS_SWAFO.ViewModels
{
    public class CreateStudentViewModel
    {
        public StudentsRecordModel NewStudent { get; set; }
        [ValidateNever]
        public IQueryable<StudentsRecordModel> ExistingStudents { get; set; }
    }
}
