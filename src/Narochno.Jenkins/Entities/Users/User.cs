using System;

namespace Narochno.Jenkins.Entities.Users
{
    public class User
    {
        public Uri AbsoluteUrl { get; set; }
        public string FullName { get; set; }

        public override string ToString() => FullName;
    }
}
