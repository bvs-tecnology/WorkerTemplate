using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
