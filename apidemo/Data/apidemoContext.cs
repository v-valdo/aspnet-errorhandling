using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apidemo.Models;

namespace apidemo.Data;
public class apidemoContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public apidemoContext(DbContextOptions<apidemoContext> options) : base(options)
    {
    }
}