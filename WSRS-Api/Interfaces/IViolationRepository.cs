using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Dtos;

namespace WSRS_Api.Interfaces;

public interface IViolationRepository
{
    Task<AllReportsDto> GetByIdAsync(int studentNumber);
    Task<bool> IsStudentClear(int studentNumber);
}
