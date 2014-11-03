using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanssens.Net.Identity.RavenDb.Models
{
    /// <summary>
    /// ViewModel for handling the user's login.
    /// </summary>
    public class Login
    {
        [Required(ErrorMessage = "Gebruikersnaam is vereist")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Wachtwoord is vereist")]
        public string Password { get; set; }

    }
}
