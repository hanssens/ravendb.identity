using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanssens.Net.Identity.RavenDb.Models
{
    public class RavenDbUser : System.Web.Security.MembershipUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string AccountConfirmationToken { get; set; }
        public string PasswordToken { get; set; }
        public DateTime TimeValidPasswordToken { get; set; }
        public DateTime? PasswordLastChangedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public RavenDbUser()
        {
            Attributes = new Dictionary<string, object>();
            Roles = new List<string>();
        }
    }
}
