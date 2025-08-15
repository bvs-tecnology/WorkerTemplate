using System.Diagnostics.CodeAnalysis;

namespace Infra.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork(Data.Context.Context context) : IUnitOfWork, IDisposable
    {
        public Data.Context.Context Context { get; set; } = context;

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                Context.Dispose();
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

