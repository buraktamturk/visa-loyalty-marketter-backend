using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Visa.Marketter.Models;
using Visa.Marketter.Utils;

namespace Visa.Marketter.Controllers {
    public class Campaign {
        public int? id { get; set; }

	    public string name { get; set; }
        
	    public int? visa_offer_id { get; set; }
	    
	    public int? card_type { get; set; }
        
	    public string template_id { get; set; }
        
        public DateTimeOffset? created_at { get; set; }
	    
	    public DateTimeOffset? updated_at { get; set; }
	    
	    public List<CampaignCustomer> users { get; set; }
    }

	public class CampaignCustomer {
		
		public Customer customer { get; set; }
		
		public string video_id { get; set; }
		
		public JToken video { get; set; }
		
	}

    public class CampaignController : Controller {
        private readonly ApplicationDbContext db;

        public static readonly Expression<Func<campaign, Campaign>> campaign = a => new Campaign {
            id = a.id,
            name = a.name,
            visa_offer_id = a.visa_offer_id,
	        created_at = a.created_at,
	        updated_at = a.updated_at,
	        card_type = a.card_type,
	        template_id = a.template_id,
	        users = a.customers
	                 .Select(b => new CampaignCustomer() {
						customer = CustomerController.customer.Invoke(b.customer),
		                 video_id = b.render_id
					})
	                 .ToList()
        };

	    static CampaignController() {
		    campaign = campaign.Expand();
	    }

        public CampaignController(ApplicationDbContext db) {
            this.db = db;
        }

        [HttpGet("campaign")]
        public async Task<List<Campaign>> all([FromServices] Token token) {
	        if (token.type != 512) {
		        throw new Exception("you need to be an admin");
	        }
	        
            return await db.campaigns
                .Select(campaign)
                .ToListAsync();
        }

        [HttpGet("campaign/{id}")]
        public async Task<Campaign> single([FromServices] Token token, int? id) {
	        if (token.type != 512) {
		        throw new Exception("you need to be an admin");
	        }
	        
            var data = await db.campaigns
                .Where(a => a.id == id)
                .Select(campaign)
                .SingleAsync();
	        
	        using (var client = new HttpClient()) {
		        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("acc_790C1532284E2E48B7C01ED2D0F68B46:555A52148E683418E59BE26E7ACB444D68C6F5881D5AFC89C68FAAB77CB3C366")));

		        foreach(var customer in data.users) {
			        using (var request = await client.GetAsync($"https://api.vidimake.com/brand/visa/render/{customer.video_id}")) {
				        var response = await request.Content.ReadAsStringAsync();
				        var obj = JObject.Parse(response);

				        customer.video = obj;
			        }
		        }
	        }

	        return data;
        }
	    
	    [HttpDelete("campaign/{id}")]
	    public async Task delete_user([FromServices] Token token, int? id) {
		    if (token.type != 512) {
			    throw new Exception("you need to be an admin");
		    }

		    db.campaigns
			   .RemoveRange(db.campaigns.Where(a => a.id == id));

		    await db.SaveChangesAsync();
	    }

	    [HttpPut("campaign")]
	    [HttpPatch("campaign/{id}")]
	    public async Task<Campaign> update_user([FromServices] Token token, int? id, [FromBody] Campaign Campaign) {
		    campaign campaign;

		    if (id == null) {
			    if (token.type != 512) {
				    throw new Exception("you need to be an admin");
			    }
			    
			    campaign = new campaign {
				    created_at = DateTimeOffset.Now,
				    customers = new List<campaign_customer>()
			    };

			    db.campaigns.Add(campaign);
			    
			    /*
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
			    */

			    var customers = await db.customer.ToListAsync();

			    using (var client = new HttpClient()) {
				    client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("acc_790C1532284E2E48B7C01ED2D0F68B46:555A52148E683418E59BE26E7ACB444D68C6F5881D5AFC89C68FAAB77CB3C366")));

				    foreach(var customer in customers) {
						using (var request = await client.PutAsync("https://api.vidimake.com/brand/visa/template/visa/render", new StringContent(
							JsonConvert.SerializeObject(new {
								body = new {
									text1 = customer.name
								},
								email = customer.email
							}), Encoding.UTF8, "application/json"))) {
							var response = await request.Content.ReadAsStringAsync();
							var obj = JObject.Parse(response);
							
							campaign.customers.Add(new campaign_customer() {
								customer_id = customer.id,
								render_id = obj["_id"].Value<string>(),
								status = 0
							});
						}
				    }
			    }
		    } else {
			    if (token.type != 512) {
				    throw new Exception("you need to be an admin");
			    }
			    
			    campaign = await db.campaigns
			                       .Include(a => a.customers)
			                       .SingleAsync(a => a.id == id);
			    
			    campaign.updated_at = DateTimeOffset.Now;
		    }
			
		    if (Campaign.name != null && campaign.name != Campaign.name) {
			    campaign.name = Campaign.name;
		    }
		    
		    if (Campaign.visa_offer_id != null && campaign.visa_offer_id != Campaign.visa_offer_id) {
			    campaign.visa_offer_id = Campaign.visa_offer_id.Value;
		    }
		    
		    if (Campaign.template_id != null && campaign.template_id != Campaign.template_id) {
			    campaign.template_id = Campaign.template_id;
		    }
		    
		    if (Campaign.card_type != null && campaign.card_type != Campaign.card_type) {
			    campaign.card_type = Campaign.card_type.Value;
		    }
		    
			await db.SaveChangesAsync();

		    return await single(token, campaign.id);
	    }
    }
}