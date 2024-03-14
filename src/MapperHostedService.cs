using Microsoft.Extensions.Hosting;

namespace Lycoris.AutoMapper.Extensions
{
    internal class MapperHostedService : IHostedService
    {
        public MapperHostedService(IServiceProvider serviceProvider)
        {
            AutoMapperExtensions.SetAutoMapperServiceProvider(serviceProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
