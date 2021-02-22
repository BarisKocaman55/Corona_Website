using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Corono_Website.Models.Manager
{
    public class DataBaseContext_Admin : DbContext
    {
        public DataBaseContext_Admin() : base("DataBaseContext_Admin") { }
        public DbSet<Admins> Admins { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admins>().ToTable("Admins");
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}