using Domain.SeedWork.Notification;
using Microsoft.AspNetCore.Http;

namespace Application
{
    public class ServiceProviderProxy(IHttpContextAccessor httpContextAccessor) : IContainer
    {
        public T GetService<T>() => (T)httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T))!;
    }
}
