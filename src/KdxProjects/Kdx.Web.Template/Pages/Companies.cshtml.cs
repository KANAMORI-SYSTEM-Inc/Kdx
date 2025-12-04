using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kdx.Web.Template.Pages
{
    public class CompaniesModel : PageModel
    {
        private readonly ISupabaseRepository _repository;
        private readonly ILogger<CompaniesModel> _logger;

        public List<Company>? Companies { get; set; }
        public string? ErrorMessage { get; set; }

        public CompaniesModel(ISupabaseRepository repository, ILogger<CompaniesModel> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Fetching companies from Supabase...");
                Companies = await _repository.GetCompaniesAsync();
                _logger.LogInformation("Successfully fetched {Count} companies", Companies?.Count ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching companies");
                ErrorMessage = $"Failed to fetch companies: {ex.Message}";
            }
        }
    }
}
