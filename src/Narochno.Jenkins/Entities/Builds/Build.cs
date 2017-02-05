using System;

namespace Narochno.Jenkins.Entities.Builds
{
    public class Build
    {
        public long Number { get; set; }
        public Uri Url { get; set; }

        public override string ToString() => Number.ToString();
    }
}
