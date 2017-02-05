using System;

namespace Narochno.Jenkins.Entities.Jobs
{
    public class Job
    {
        public string Color { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }

        public override string ToString() => Name;
    }
}
