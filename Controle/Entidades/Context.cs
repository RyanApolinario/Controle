using Controle;
using Controle.NovaPasta;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace entidades
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            base.Database.SetConnectionString(Global.StringConexao);
            base.Database.OpenConnection();
        }
        public DbSet<Cliente> Cliente { get; set; }

        public Context() : base()
        {
        }
    }
}
