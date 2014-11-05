﻿using System;
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
        public ActionResult Login()
        {
            var model = new Login();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (!ModelState.IsValid) return View("Login", model);

            // Verify the provided credentials
            var authorized = WebSecurity.Login(model.Username, model.Password, persistCookie: false);

            if (!authorized)
            {
                ModelState.AddModelError("Username", "Authorization_InvalidUsernamePasswordCombination");
                return View("Login", model);
            }

            return Redirect("~/");
        }

        [Authorize]
        public ActionResult Logout()
        {
            WebSecurity.Logout();
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}