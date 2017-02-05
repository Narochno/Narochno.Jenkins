using Narochno.Jenkins.Entities.Jobs;
using System.Collections.Generic;

namespace Narochno.Jenkins.Entities.Views
{
    public class ViewInfo : View
    {
        public string Description { get; set; }
        public IList<Job> Jobs { get; set; } = new List<Job>();
    }
}
