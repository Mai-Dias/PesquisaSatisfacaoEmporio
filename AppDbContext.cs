using Microsoft.EntityFrameworkCore;

namespace PesquisaSatisfacaoEmporio
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
    }

    public class SurveyResponse
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool Consent { get; set; }
        public string CPF { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
