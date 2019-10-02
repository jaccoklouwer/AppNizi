using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    class Account
    {
        // Waarschijnlijk kunnen wachtwoord en salt weg. Want die hoeven we in applicatie niet op te slaan. 
        // Hierdoor moeten we misschien wel meerdere modellen voor Accounts hebben omdat ze wel mee gestuurt moeten worden
        // Met het inloggen.
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
    }
}
