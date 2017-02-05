using Narochno.Jenkins.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Narochno.Jenkins.Entities.Builds
{
    public class ChangeSetItem
    {
        public string CommitId { get; set; }
        public string ShortCommitId => CommitId.Length > 8 ? CommitId.Substring(0, 8) : CommitId;
        public DateTime Date { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        public User Author { get; set; }
        public IList<string> AffectedPaths { get; set; } = new List<string>();
        public IList<ChangeSetPath> Paths { get; set; } = new List<ChangeSetPath>();

        public override string ToString() => $"{ShortCommitId}: {Message} - {Author}";
    }
}