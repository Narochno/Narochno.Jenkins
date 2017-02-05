using System.Threading.Tasks;
using System;
using System.Threading;
using Narochno.Jenkins.Entities;
using Narochno.Jenkins.Entities.Builds;
using Narochno.Jenkins.Entities.Jobs;
using Narochno.Jenkins.Entities.Views;
using Narochno.Jenkins.Entities.Users;

namespace Narochno.Jenkins
{
    public interface IJenkinsClient : IDisposable
    {
        Task<BuildInfo> GetBuild(string job, string build, CancellationToken ctx = default(CancellationToken));
        Task<JobInfo> GetJob(string job, CancellationToken ctx = default(CancellationToken));
        Task<UserInfo> GetUser(string user, CancellationToken ctx = default(CancellationToken));
        Task<ViewInfo> GetView(string view, CancellationToken ctx = default(CancellationToken));
        Task<Master> GetMaster(CancellationToken ctx = default(CancellationToken));
        Task BuildProject(string job, CancellationToken ctx = default(CancellationToken));
    }
}