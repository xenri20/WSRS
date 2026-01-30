using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using WSRS_Api.Data;
using WSRS_Api.Models;

namespace WSRS_Api.Repositories
{
    public class GmcRepository
    {
        private ApplicationDbContext _context;
        private readonly ILogger<GmcRepository> _logger;

        public GmcRepository(ILogger<GmcRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IEnumerable<GoodMoralRequest> GetAll()
        {
            try
            {
                var requests = _context.GoodMoralRequests
                    .AsNoTracking()
                    .ToList();

                return requests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return new List<GoodMoralRequest>();
        }

        public GoodMoralRequest? GetById(int id)
        {
            try
            {
                var request = _context.GoodMoralRequests.Find(id);
                if (request != null) return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }

        public GoodMoralRequest? PostGoodMoralRequest(GoodMoralRequest request)
        {
            try
            {
                _context.GoodMoralRequests.Add(request);
                _context.SaveChanges();

                return request;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return null;
        }

        public async Task UpdateGoodMoralRequestPatch(int id, JsonPatchDocument<GoodMoralRequest> request)
        {
            var existingRequest = GetById(id);
            if (existingRequest != null)
            {
                request.ApplyTo(existingRequest);
                await _context.SaveChangesAsync();
            }
        }
    }
}
