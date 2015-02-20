using System.Security;
using Hanssens.Net.Identity.RavenDb.Models;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Configuration;
using Raven.Client.Linq;

namespace Hanssens.Net.Identity.RavenDb
{
    public class RavenDbRoleProvider : RoleProvider, IDisposable
    {
        protected DocumentStore DataContext { get; set; }
        protected IDocumentSession CurrentSession { get; set; }

        public RavenDbRoleProvider() : this(new DocumentStore()) { }

        public RavenDbRoleProvider(DocumentStore documentStore)
        {
            DataContext = documentStore;

            // Initialize
            DataContext.Url = ConfigurationManager.AppSettings["ServerPath"];
            DataContext.DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"];
            DataContext.Initialize();


            CurrentSession = DataContext.OpenSession();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var session = DataContext.OpenSession())
            {
                var users = session.Query<RavenDbUser>().Where(u => u.Username.In(usernames));
                foreach (var user in users)
                {
                    var roles = user.Roles.ToList();
                    roles.AddRange(roleNames);

                    user.Roles = roles.Distinct();
                }

                session.SaveChanges();
            }

            foreach (var username in usernames)
            {
                
            }
            
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (RoleExists(roleName)) return;

            var role = new RavenDbRole()
            {
                Name = roleName
            };

            CurrentSession.Store(role);
            CurrentSession.SaveChanges();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var roles = new List<string>();
            using (var session = DataContext.OpenSession())
            {
                var user = session.Query<RavenDbUser>().FirstOrDefault(u => u.Username == username);
                if (user == null) throw new Exception("User not found");

                roles = user.Roles.ToList();
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var user = CurrentSession.Query<RavenDbUser>().SingleOrDefault(u => u.Username == username);
            if (user == null) throw new SecurityException("User does not exist");

            return user.Roles.Any(r => r == roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return CurrentSession.Query<RavenDbRole>().Any(r => r.Name == roleName);
        }

        public void Dispose()
        {
            CurrentSession.Dispose();
            DataContext.Dispose();
        }
    }
}
