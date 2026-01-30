using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WSRS_Student.Interface;
using WSRS_Student.Models;

namespace WSRS_Student.Service
{
    public class StudentService : IStudentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager, ILogger<StudentService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> HandleStudentStatus(ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);
            var studentNumber = user?.StudentNumber;
            var client = _httpClientFactory.CreateClient("WSRS_Api");
            var response = await client.GetAsync($"violations/is-clear/{studentNumber}");
            var data = await response.Content.ReadFromJsonAsync<bool>();
            return data;
        }
    }
}
