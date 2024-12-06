namespace WSRS_SWAFO.ViewModels
{
    public class CreateStudentViewModel
    {
        public StudentsRecordModel NewStudent { get; set; }
        public IQueryable<StudentsRecordModel> ExistingStudents { get; set; }
    }
}
