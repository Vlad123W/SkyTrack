using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal abstract class Authorization
    {
        public User? user;

        public Authorization(string login, string password)
        {
            user = new User
            {
                Login = login,
                Password = Hasher.GetSha256Hash(password),
                IsAdmin = false
            };
        }

        public abstract bool LogIn();
    }
}
