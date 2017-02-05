using Narochno.Primitives.Json;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Narochno.Jenkins.Entities;
using Newtonsoft.Json.Converters;
using System.Net.Http.Headers;
using System.Text;
using Polly.Retry;
using Polly;
using Narochno.Jenkins.Entities.Builds;
using Narochno.Jenkins.Entities.Jobs;
using Narochno.Jenkins.Entities.Views;
using Narochno.Jenkins.Entities.Users;

namespace Narochno.Jenkins
{
    public class JenkinsClient : IJenkinsClient
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly JenkinsConfig jenkinsConfig;
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[] { new OptionalJsonConverter(), new StringEnumConverter() }
        };

        public JenkinsClient(JenkinsConfig jenkinsConfig)
        {
            if (jenkinsConfig == null)
            {
                throw new ArgumentNullException(nameof(jenkinsConfig));
            }

            this.jenkinsConfig = jenkinsConfig;

            if (jenkinsConfig.Username.HasValue && jenkinsConfig.ApiKey.HasValue)
            {
                var byteArray = Encoding.ASCII.GetBytes(jenkinsConfig.Username.Value + ':'  + jenkinsConfig.ApiKey.Value);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public async Task<UserInfo> GetUser(string user, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/user/" + WebUtility.UrlEncode(user) + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<UserInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<ViewInfo> GetView(string view, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/view/" + WebUtility.UrlEncode(view) + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ViewInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<BuildInfo> GetBuild(string job, string build, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/job/" + WebUtility.UrlEncode(job) + "/" + WebUtility.UrlEncode(build) + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<BuildInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<JobInfo> GetJob(string job, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/job/" + WebUtility.UrlEncode(job) + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<JobInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<Master> GetMaster(CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Master>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task BuildProject(string job, CancellationToken ctx = default(CancellationToken))
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/job/" + WebUtility.UrlEncode(job) + "/build", null));

            response.EnsureSuccessStatusCode();
        }

        public RetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(jenkinsConfig.RetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(jenkinsConfig.RetryBackoffExponent, retryAttempt)));
        }

        public void Dispose() => httpClient.Dispose();
    }
}