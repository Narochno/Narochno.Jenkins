using Narochno.Jenkins.Entities.Users;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Narochno.Jenkins.Entities.Builds
{
    public class BuildInfo : Build
    {
        public bool Building { get; set; }
        public string BuiltOn { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public long Duration { get; set; }
        public long EstimatedDuration { get; set; }
        public JObject Executor { get; set; }
        public string FullDisplayName { get; set; }
        public string Id { get; set; }
        public bool KeepLog { get; set; }
        public long QueuedId { get; set; }
        public string Result { get; set; }
        public ChangeSet ChangeSet { get; set; }
        public IList<User> Culprits { get; set; } = new List<User>();
        public JArray Actions { get; set; }

        public override string ToString() => FullDisplayName;
    }
}