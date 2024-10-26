using Microsoft.EntityFrameworkCore;
using TeaWork.Data;

namespace TeaWork.Logic.DbContextFactory
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DbContextFactory(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }
    }
}
