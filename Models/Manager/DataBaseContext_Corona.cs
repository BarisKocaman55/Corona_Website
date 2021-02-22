using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Corono_Website.Models.Manager
{
    public class DataBaseContext_Corona : DbContext
    {
        public DataBaseContext_Corona() : base("DataBaseContext_Corona") { }
        public DbSet<CoronaTable> CoronaTable { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoronaTable>().ToTable("CoronaTable");
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}