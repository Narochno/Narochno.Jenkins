using Newtonsoft.Json.Linq;

namespace Narochno.Jenkins.Entities.Users
{
    public class UserInfo : User
    {
        public string Description { get; set; }
        public string Id { get; set; }

        public JArray Property { get; set; }
    }
}
