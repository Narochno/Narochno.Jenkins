using System.Collections.Generic;

namespace Narochno.Jenkins.Entities.Builds
{
    public class ChangeSet
    {
        public string Kind { get; set; }
        public IList<ChangeSetItem> Items { get; set; } = new List<ChangeSetItem>();
    }
}
