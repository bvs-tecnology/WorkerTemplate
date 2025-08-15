namespace Infra.Data.Context
{
    public interface IUnitOfWork
    {
        Data.Context.Context Context { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
