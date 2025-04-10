using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal abstract class Authorization
    {
        private string? login;
        public string Login { get => login!; set => login = value; }

        private string? password;
        public string Password { get => password!; set => password = value; }

        public Authorization(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public abstract bool LogIn();
    }
}
