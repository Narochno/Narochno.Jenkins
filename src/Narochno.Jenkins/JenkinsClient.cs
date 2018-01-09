using Narochno.Primitives.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using Narochno.Primitives;
using System.Linq;

namespace Narochno.Jenkins
{
    public class JenkinsClient : IJenkinsClient
    {
        private readonly HttpClient httpClient;
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

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            httpClient = new HttpClient(handler);

            this.jenkinsConfig = jenkinsConfig;

            if (jenkinsConfig.Username.HasValue && jenkinsConfig.ApiKey.HasValue)
            {
                var byteArray = Encoding.ASCII.GetBytes(jenkinsConfig.Username.Value + ':'  + jenkinsConfig.ApiKey.Value);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public async Task<UserInfo> GetUser(string user, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/user/" + user + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<UserInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<ViewInfo> GetView(string view, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/view/" + view + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ViewInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<BuildInfo> GetBuild(string job, string build, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/" + build + "/api/json", ctx));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<BuildInfo>(await response.Content.ReadAsStringAsync(), serializerSettings);
        }

        public async Task<JobInfo> GetJob(string job, CancellationToken ctx)
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/api/json", ctx));

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
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/build", null));

            response.EnsureSuccessStatusCode();
        }

        public async Task BuildProjectWithParameters(string job, IDictionary<string, string> parameters, CancellationToken ctx = default(CancellationToken))
        {
            var response = await GetRetryPolicy().ExecuteAsync(() =>
            {
                var p = new {parameter = parameters.Select(x => new {name = x.Key, value = x.Value}).ToArray()};
                var json = JsonConvert.SerializeObject(p);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("json", json)
                });

                return httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/build", content, ctx);
            });

            response.EnsureSuccessStatusCode();
        }

        public async Task CopyJob(string fromJobName, string newJobName, CancellationToken ctx = default(CancellationToken))
        {
            var requestUri = jenkinsConfig.JenkinsUrl + "/createItem" + $"?name={newJobName}&mode=copy&from={fromJobName}";
            var content = new StringContent("", Encoding.UTF8, "application/xml");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            };

            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.SendAsync(message, ctx));

            response = await FollowRedirect(response);
            
            response.EnsureSuccessStatusCode();
        }

        private async Task<HttpResponseMessage> FollowRedirect(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.Redirect) return response;

            return await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(response.Headers.Location.AbsoluteUri));
        }

        public async Task<string> DownloadJobConfig(string job, CancellationToken ctx = default(CancellationToken))
        {
            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/config.xml", ctx));

            response.EnsureSuccessStatusCode();

            var config = await response.Content.ReadAsStringAsync();

            return config;
        }

        public async Task UploadJobConfig(string job, string xml, CancellationToken ctx = default(CancellationToken))
        {
            var content = new StringContent(xml, Encoding.UTF8, "application/xml");

            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/config.xml", content, ctx));

            response.EnsureSuccessStatusCode();
        }

        public async Task EnableJob(string job, CancellationToken ctx = default(CancellationToken))
        {
            var content = new StringContent("");

            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/enable", content, ctx));

            response = await FollowRedirect(response);

            response.EnsureSuccessStatusCode();
        }

        public async Task DisableJob(string job, CancellationToken ctx = default(CancellationToken))
        {
            var content = new StringContent("");

            var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/job/" + job + "/disable", content, ctx));

            response = await FollowRedirect(response);

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