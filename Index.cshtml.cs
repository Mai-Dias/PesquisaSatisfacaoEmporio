using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PesquisaSatisfacaoEmporio.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PesquisaSatisfacaoEmporio.AppDbContext _db;

        public IndexModel(PesquisaSatisfacaoEmporio.AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public int Step { get; set; } = 1;

        [BindProperty]
        public int Rating { get; set; }

        [BindProperty]
        public string Comment { get; set; }

        [BindProperty]
        public bool Consent { get; set; }

        [BindProperty]
        public string CPF { get; set; }

        public string FinalMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Step < 4)
            {
                Step++;
                return Page();
            }
            if (Step == 4)
            {
                // Validação extra de CPF (com máscara)
                string cpfNum = Regex.Replace(CPF ?? "", @"\D", "");
                if (cpfNum.Length != 11)
                {
                    ModelState.AddModelError("CPF", "CPF inválido");
                    return Page();
                }

                // Salva resposta no banco
                var resp = new PesquisaSatisfacaoEmporio.SurveyResponse
                {
                    Rating = Rating,
                    Comment = Comment,
                    Consent = Consent,
                    CPF = cpfNum
                };
                _db.SurveyResponses.Add(resp);
                await _db.SaveChangesAsync();

                // Integração Bonifiq
                var sucesso = await CreditarBonifiqAsync(cpfNum);
                if (sucesso)
                {
                    Step = 5;
                    FinalMessage = "Obrigado! Sua participação foi registrada e o benefício já foi liberado no seu CPF.";
                }
                else
                {
                    Step = 5;
                    FinalMessage = "Sua avaliação foi salva, mas houve um erro ao liberar o benefício. Tente novamente ou fale com nossa equipe.";
                }
                return Page();
            }
            return Page();
        }

        private async Task<bool> CreditarBonifiqAsync(string cpf)
        {
            var client = new HttpClient();
            var payload = new
            {
                cpf = cpf,
                objectiveCode = "3893",
                amount = 1
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Add("apiuser", "APIUSER-EmprioQuat-d622af2ac1364f8cab0520cbedd0ba93");
            client.DefaultRequestHeaders.Add("apipass", "73PD332YKV3K8J53C5HS84ESMT3Q9C");

            try
            {
                var response = await client.PostAsync("https://api.bonifiq.com.br/v1/objectives/credit-by-cpf", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
