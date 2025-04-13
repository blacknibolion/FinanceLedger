using Microsoft.EntityFrameworkCore;

public class LedgerDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    //LoclaDB connection string
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FinanceLedgerDb;Trusted_Connection=True;");
    }
}



