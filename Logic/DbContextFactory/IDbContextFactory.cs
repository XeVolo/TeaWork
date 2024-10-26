using TeaWork.Data;

namespace TeaWork.Logic.DbContextFactory
{
    public interface IDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
    }
}
