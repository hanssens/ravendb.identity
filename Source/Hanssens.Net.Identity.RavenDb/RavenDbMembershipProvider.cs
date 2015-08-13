using System.Configuration;
using System.Data;
using System.Security;
using System.Web.Security;
using Hanssens.Net.Cryptography;
using Hanssens.Net.Identity.RavenDb.Models;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace Hanssens.Net.Identity.RavenDb
{
    public class RavenDbMembershipProvider : ExtendedMembershipProvider, IDisposable
    {
        public DocumentStore DataContext { get; set; }
        protected IDocumentSession CurrentSession { get; set; }

        public RavenDbMembershipProvider() : this(new DocumentStore()) { }

        public RavenDbMembershipProvider(DocumentStore documentStore)
        {
            DataContext = documentStore;

            // Initialize
            DataContext.Url = ConfigurationManager.AppSettings["ServerPath"];
            DataContext.DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"];
            DataContext.Initialize();


            CurrentSession = DataContext.OpenSession();
        }

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            return CreateUserAndAccount(userName, password, requireConfirmationToken, null);
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            RavenQueryStatistics stats;
            var exists = CurrentSession.Query<RavenDbUser>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Any(u => u.Username == userName);

            if (exists) throw new DuplicateNameException("User already exists");

            // hash the password
            var hashedPassword = PasswordHash.CreateHash(password);

            var user = new RavenDbUser()
            {
                CreatedOn = DateTime.Now,
                Username = userName,
                Password = hashedPassword
            };

            CurrentSession.Store(user);
            CurrentSession.SaveChanges();

            return user.Username;
        }

        public override bool DeleteAccount(string userName)
        {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            RavenDbUser user = null;

            using (var session = DataContext.OpenSession())
            {
                RavenQueryStatistics stats;
                user = session.Query<RavenDbUser>()
                    .Statistics(out stats)
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                    .SingleOrDefault(u => u.Username == userName);

                var original = session.Load<RavenDbUser>(user.Id);
                session.Delete(original);
                session.SaveChanges();

                user.PasswordToken = Guid.NewGuid().ToString("N");

                session.Store(user);
                session.SaveChanges();
            }

            return user.PasswordToken;
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            try
            {
                RavenDbUser user;
                using (var session = DataContext.OpenSession())
                {
                    RavenQueryStatistics stats;
                    user = session.Query<RavenDbUser>()
                        .Statistics(out stats)
                        .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                        .SingleOrDefault(u => u.PasswordToken == token);

                    if (user == null)
                        throw new Exception("Couldnt find user");

                    user.Password = PasswordHash.CreateHash(newPassword);

                    var original = session.Load<RavenDbUser>(user.Id);
                    session.Delete(original);
                    session.SaveChanges();

                    user.PasswordToken = null;

                    session.Store(user);
                    session.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = CurrentSession.Query<RavenDbUser>()
                .Single(u => u.Username == username);

            if (!string.IsNullOrEmpty(oldPassword))
            {
                if (!PasswordHash.ValidatePassword(oldPassword, user.Password))
                    throw new Exception("Old passwords do not match");
            }

            try
            {
                // Delete old user
                CurrentSession.Delete(user);
                CurrentSession.SaveChanges();

                // Hash the new password
                var hashedPassword = PasswordHash.CreateHash(newPassword);

                // Update the user with the new hashed password
                user.Password = hashedPassword;

                // Save the updated user
                CurrentSession.Store(user);
                CurrentSession.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            status = new System.Web.Security.MembershipCreateStatus();

            // hash the password
            var hashedPassword = PasswordHash.CreateHash(password);

            var user = new RavenDbUser
            {
                CreatedOn = DateTime.Now,
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            CurrentSession.Store(user);
            CurrentSession.SaveChanges();

            return user;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var user = CurrentSession.Query<RavenDbUser>()
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(2)))
                    .FirstOrDefault(u => u.Username.Equals(username));

            try
            {
                var entity = CurrentSession.Load<RavenDbUser>(user.Id);
                CurrentSession.Delete(entity);
                CurrentSession.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            RavenDbUser user = null;

            using (var session = DataContext.OpenSession())
            {
                RavenQueryStatistics stats;
                user = session.Query<RavenDbUser>()
                    .Statistics(out stats)
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                    .SingleOrDefault(u => u.Username == username);
            }

            if (user == null) return false;

            return PasswordHash.ValidatePassword(password, user.Password);
        }

        public void Dispose()
        {
            CurrentSession.Dispose();
            DataContext.Dispose();
        }

        /*
         * Extensions
         */
    }
}
