using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FunctionMZ301 {
    public class SendEmailFunction {
        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log) {
            var client = new HttpClient();
            var response = await client.GetAsync("https://api.nbp.pl/api/exchangerates/rates/a/eur/").ConfigureAwait(false);
            if (response.IsSuccessStatusCode) {
                var data = await response.Content.ReadAsAsync<Root>();
                if (data.Rates.FirstOrDefault().Mid > 4.70) {
                    var smtpClient = new SmtpClient() {
                        Port = 587,
                        Credentials = new NetworkCredential("", ""),
                        EnableSsl = true,
                        Host = "smtp.gmail.com"
                    };
                    smtpClient.Send("", "", "Kurs euro", "Kurs przekroczył 4.70!");
                }
            }
        }

        public class Rate {
            public string No { get; set; }
            public string EffectiveDate { get; set; }
            public double Mid { get; set; }
        }

        public class Root {
            public string Table { get; set; }
            public string Currency { get; set; }
            public string Code { get; set; }
            public List<Rate> Rates { get; set; }
        }
    }
}
