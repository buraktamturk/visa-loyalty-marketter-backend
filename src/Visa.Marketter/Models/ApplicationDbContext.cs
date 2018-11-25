using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Visa.Marketter.Models {
    public class customer {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string card_number { get; set; }
        
        public DateTimeOffset? created_at { get; set; }
	    
        public DateTimeOffset? updated_at { get; set; }
    }
    
    public class user {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        
        public string email { get; set; }

        public byte[] password_salt { get; set; }

        public byte[] password_hash { get; set; }
		
	    public DateTimeOffset? password_set_at { get; set; }

		public int type { get; set; }
        
        public DateTimeOffset created_at { get; set; }

        public DateTimeOffset? updated_at { get; set; }
    }

    public class client {
        [Key]
        public Guid id { get; set; }

        public string client_name { get; set; }

        public byte[] client_secret_salt { get; set; }

        public byte[] client_secret_hash { get; set; }

        public bool trusted { get; set; }

		public string product_name { get; set; }

		public string copyright_text { get; set; }

		public string sender { get; set; }

        public virtual List<refresh_token> refresh_tokens { get; set; }
    }
    
    public class refresh_token {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid id { get; set; }

        public byte[] exchange_code_salt { get; set; }

        public byte[] exchange_code_hash { get; set; }

        public byte[] refresh_token_salt { get; set; }

        public byte[] refresh_token_hash { get; set; }

        public int user_id { get; set; }
        
        public Guid? client_id { get; set; }

        public virtual client client { get; set; }

        public DateTimeOffset created_at { get; set; }

        public virtual user user { get; set; }
    }

    public class personal_access_token {
        [Key]
        public Guid id { get; set; }

        public int user_id { get; set; }

        public virtual user user { get; set; }

        public string name { get; set; }

        public byte[] password_salt { get; set; }

        public byte[] password_hash { get; set; }

        public DateTimeOffset created_at { get; set; }

        public DateTimeOffset? updated_at { get; set; }

        public DateTimeOffset? deleted_at { get; set; }
    }

    public class campaign {
        [Key]
        public int id { get; set; }
        
        public string name { get; set; }
        
        public int visa_offer_id { get; set; }
        
        public int? card_type { get; set; }
        
        public string template_id { get; set; }
        
        public DateTimeOffset created_at { get; set; }
        
        public DateTimeOffset? updated_at { get; set; }
        
        public virtual List<campaign_customer> customers { get; set; }
    }

    public class campaign_customer {
        [Key]
        public int id { get; set; }
        
        public int campaign_id { get; set; }
        
        public virtual campaign campaign { get; set; }
        
        public int customer_id { get; set; }
        
        public virtual customer customer { get; set; }
        
        public int status { get; set; }
        
        public string render_id { get; set; }
        
        public DateTimeOffset? updated_at { get; set; }
    }

    public class ApplicationDbContext : DbContext {
        public virtual DbSet<customer> customer { get; set; }
        
        public virtual DbSet<client> clients { get; set; }

        public virtual DbSet<refresh_token> refresh_tokens { get; set; }

        public virtual DbSet<user> users { get; set; }

        public virtual DbSet<personal_access_token> pacs { get; set; }
        
        public virtual DbSet<campaign> campaigns { get; set; }
        
        public virtual DbSet<campaign_customer> campaign_customers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {
                property.AsProperty().Builder
                    .HasMaxLength(256, ConfigurationSource.Convention);
            }

            modelBuilder.Entity<campaign>()
                        .HasMany(a => a.customers)
                        .WithOne(a => a.campaign)
                        .HasForeignKey(a => a.campaign_id);
            
            modelBuilder.Entity<campaign_customer>()
                        .HasOne(a => a.customer)
                        .WithMany()
                        .HasForeignKey(a => a.customer_id);
        }
    }
}
