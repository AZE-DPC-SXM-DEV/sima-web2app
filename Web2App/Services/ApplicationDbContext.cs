using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web2App.Data;
using Web2App.Interfaces;
using Web2App.Models;

namespace Web2App.Services
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        private readonly AppDbContext _context;
        public ApplicationDbContext(AppDbContext context)
        {
           _context = context;

        }
        public async Task LogAsync(SimaLog model)
        {
            await _context.SimaLogs.AddAsync(model);
            await _context.SaveChangesAsync();
        }



    }
}
