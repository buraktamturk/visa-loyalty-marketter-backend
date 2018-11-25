using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Visa.Marketter.Controllers {
    public class OfferController : Controller {
        [HttpGet("offer")]
        public async Task<JToken> all() {
            using (var visaHandler = new HttpClientHandler()) {
                visaHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                visaHandler.ClientCertificates.Add(new X509Certificate2("/Users/buraktamturk/Downloads/visa.pfx", "abcd"));

                using (var http = new HttpClient(visaHandler)) {
                    http.DefaultRequestHeaders.Add("Authorization", "Basic ME85TTdFVTM1SVpEWTRDSzBaWjIyMTQtMGFIejJFYThRbGo5RDI2QjZqcm1IVFVpODpUQ2kySGltNk0=");

                    using (var request = await http.GetAsync("https://sandbox.api.visa.com/vmorc/offers/v1/all")) {
                        var response = await request.Content.ReadAsStringAsync();
                        return JObject.Parse(response)["Offers"];
                    }
                }
            }
        }
    }
}