using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WDG.Framework.DAL;

namespace FrameworkTest
{
    public class BizData : DbContextBase
    {
        public BizData()
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<BizData>(null);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Role> Roles { get; set; }
    }
}
