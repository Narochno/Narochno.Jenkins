using System.Threading.Tasks;
using System;
using System.Collections.Generic;
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
        Task<string> GetBuildConsole(string job, string build, CancellationToken ctx = default(CancellationToken));
        Task<JobInfo> GetJob(string job, CancellationToken ctx = default(CancellationToken));
        Task<UserInfo> GetUser(string user, CancellationToken ctx = default(CancellationToken));
        Task<ViewInfo> GetView(string view, CancellationToken ctx = default(CancellationToken));
        Task<Master> GetMaster(CancellationToken ctx = default(CancellationToken));
        Task BuildProject(string job, CancellationToken ctx = default(CancellationToken));
        Task BuildProjectWithParameters(string job, IDictionary<string, string> parameters, CancellationToken ctx = default(CancellationToken));
        Task CopyJob(string fromJobName, string newJobName, CancellationToken ctx = default(CancellationToken));
        Task CopyJob(string fromJobName, string newJobName, string path, CancellationToken ctx = default(CancellationToken));
        Task<string> DownloadJobConfig(string job, CancellationToken ctx = default(CancellationToken));
        Task UploadJobConfig(string job, string xml, CancellationToken ctx = default(CancellationToken));
        Task EnableJob(string job, CancellationToken ctx = default(CancellationToken));
        Task DisableJob(string job, CancellationToken ctx = default(CancellationToken));
        Task DeleteJob(string job, CancellationToken ctx = default(CancellationToken));
        Task<bool> ExistsJob(string job, CancellationToken ctx = default(CancellationToken));
        Task CreateJob(string job, string xml, CancellationToken ctx = default(CancellationToken));
        Task CreateJob(string job, string xml, string path, CancellationToken ctx = default(CancellationToken));
        Task CreateFolder(string folder, CancellationToken ctx = default(CancellationToken));
        Task CreateFolder(string folder, string path, CancellationToken ctx = default(CancellationToken));
        Task DeleteFolder(string folder, CancellationToken ctx = default(CancellationToken));
        Task QuietDown(string reason = "", CancellationToken ctx = default(CancellationToken));
        Task CancelQuietDown(CancellationToken ctx = default(CancellationToken));
        Task Restart(CancellationToken ctx = default(CancellationToken));
        Task SafeRestart(CancellationToken ctx = default(CancellationToken));
    }
}