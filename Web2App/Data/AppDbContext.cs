using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2App.Models;

namespace Web2App.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<SimaLog> SimaLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions):base(dbContextOptions)
        {

        } 
    }
}
