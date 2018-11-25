using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Visa.Marketter.Utils;
using Microsoft.EntityFrameworkCore;
using Visa.Marketter.Models;

namespace Visa.Marketter.Controllers {
    public class Customer {
        public int? id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

		public string card_number { get; set; }

        public DateTimeOffset? created_at { get; set; }
	    
	    public DateTimeOffset? updated_at { get; set; }
    }

    public class CustomerController : Controller {
        private readonly ApplicationDbContext db;

        public static readonly Expression<Func<customer, Customer>> customer = a => new Customer {
            id = a.id,
            name = a.name,
            email = a.email,
	        card_number = a.card_number,
	        created_at = a.created_at,
	        updated_at = a.updated_at
        };

        public CustomerController(ApplicationDbContext db) {
            this.db = db;
        }

        [HttpGet("customer")]
        public async Task<List<Customer>> all([FromServices] Token token) {
	        if (token.type != 512) {
		        throw new Exception("you need to be an admin");
	        }
	        
            return await db.customer
                .Select(customer)
                .ToListAsync();
        }

        [HttpGet("customer/{id}")]
        public async Task<Customer> single_user([FromServices] Token token, int? id) {
	        if (token.type != 512) {
		        throw new Exception("you need to be an admin");
	        }
	        
            return await db.customer
                .Where(a => a.id == id)
                .Select(customer)
                .SingleAsync();
        }
	    
	    [HttpDelete("customer/{id}")]
	    public async Task delete_user([FromServices] Token token, int? id) {
		    if (token.type != 512) {
			    throw new Exception("you need to be an admin");
		    }

		    db.customer
			   .RemoveRange(db.customer.Where(a => a.id == id));

		    await db.SaveChangesAsync();
	    }

	    [HttpPut("customer")]
	    [HttpPatch("customer/{id}")]
	    public async Task<Customer> update_user([FromServices] Token token, int? id, [FromBody] Customer Customer) {
		    customer customer;

		    if (id == null) {
			    if (token.type != 512) {
				    throw new Exception("you need to be an admin");
			    }
			    
			    customer = new customer {
				    created_at = DateTimeOffset.Now
			    };

			    db.customer.Add(customer);
		    } else {
			    if (token.type != 512 && id != token.user_id) {
				    throw new Exception("you need to be an admin");
			    }
			    
			    customer = await db.customer.SingleAsync(a => a.id == id);
			    customer.updated_at = DateTimeOffset.Now;
		    }
			
		    if (Customer.name != null && customer.name != Customer.name) {
			    customer.name = Customer.name;
		    }
		    
		    if (Customer.email != null && customer.email != Customer.email) {
			    customer.email = Customer.email;
		    }
		    
		    if (Customer.card_number != null && customer.card_number != Customer.card_number) {
			    customer.card_number = Customer.card_number;
		    }

			await db.SaveChangesAsync();

		    return await single_user(token, customer.id);
	    }
    }
}
