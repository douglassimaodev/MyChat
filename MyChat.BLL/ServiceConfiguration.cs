using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MyChat.BLL
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddMyChatBLL(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ServiceConfiguration).GetTypeInfo().Assembly);

            return services;
        }
    }
}
