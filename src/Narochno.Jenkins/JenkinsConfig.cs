using Narochno.Primitives;

namespace Narochno.Jenkins
{
    public class JenkinsConfig
    {
        /// <summary>
        /// The Jenkins base URL
        /// </summary>
        public string JenkinsUrl { get; set; }

        /// <summary>
        /// The username to authenticate with
        /// </summary>
        public Optional<string> Username { get; set; }

        /// <summary>
        /// The API key for the supplied user
        /// </summary>
        public Optional<string> ApiKey { get; set; }

        /// <summary>
        /// The number of times requests will be retried 
        /// </summary>
        public int RetryAttempts { get; set; } = 2;

        /// <summary>
        /// The number of retries will be an exponent of this number
        /// </summary>
        public int RetryBackoffExponent { get; set; } = 2;
    }
}
