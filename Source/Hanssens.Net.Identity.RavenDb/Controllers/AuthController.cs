using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hanssens.Net.Identity.RavenDb.Models;
using WebMatrix.WebData;

namespace Hanssens.Net.Identity.RavenDb.Controllers
{
    /// The AuthController is responsible for logging in and logging off the users.
    /// </summary>
    [AllowAnonymous]
    public class AuthController : Controller
    {
        [HttpGet]
        public virtual ActionResult Login()
        {
            var model = new Login();
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Login(Login model)
        {
            if (!ModelState.IsValid) return View("Login", model);

            // Verify the provided credentials
            var authorized = WebSecurity.Login(model.Username, model.Password, persistCookie: false);

            if (!authorized)
            {
                OnLoginFailed();
                ModelState.AddModelError("Username", "Authorization_InvalidUsernamePasswordCombination");
                return View("Login", model);
            }

            OnLoginSucceeded();
            return Redirect("~/");
        }

        protected virtual void OnLoginSucceeded()
        {
            // Empty by design
        }

        protected virtual void OnLoginFailed()
        {
            // Empty by design
        }

        [Authorize]
        public virtual ActionResult Logout()
        {
            WebSecurity.Logout();
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
