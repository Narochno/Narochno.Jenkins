using Microsoft.Extensions.DependencyInjection;

namespace Narochno.Jenkins
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the Jenkins client to the service collection as a singleton.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="config">The jenkins configuration.</param>
        /// <returns>The passed service collection.</returns>
        public static IServiceCollection AddJenkins(this IServiceCollection services, JenkinsConfig config)
        {
            return services.AddSingleton<IJenkinsClient>(new JenkinsClient(config));
        }
    }
}
