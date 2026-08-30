using Microsoft.EntityFrameworkCore;
using MenuApi.Models;

namespace MenuApi.Data;

public class MenuDbContext : DbContext
{
    public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options) { }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
}
