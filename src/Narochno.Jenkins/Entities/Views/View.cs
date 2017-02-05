using System;

namespace Narochno.Jenkins.Entities.Views
{
    public class View
    {
        public string Name { get; set; }
        public Uri Url { get; set; }

        public override string ToString() => Name;
    }
}
