using ItemStorage.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItemStorage.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Init();
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<RequestResponseLog> RequestResponseLogs { get; set; }

    private void Init()
    {
        Database.EnsureCreated();
    }
}