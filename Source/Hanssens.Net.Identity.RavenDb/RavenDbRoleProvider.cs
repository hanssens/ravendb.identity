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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            return CurrentSession.Query<RavenDbRoles>().Any(r => r.Name == roleName);
        }

        public void Dispose()
        {
            CurrentSession.Dispose();
            DataContext.Dispose();
        }
    }
}
