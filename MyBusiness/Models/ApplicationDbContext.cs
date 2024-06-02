using Microsoft.EntityFrameworkCore;
using MyBusiness.Models.Entities;

namespace MyBusiness.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Deal> Deals { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}