using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal class Registration : Authorization
    {
        public Registration(string login, string password) : base(login, password)
        {
        }

        public override bool LogIn()
        {
            SqlQuery query = new("skytrack");
            var checker = query.GetUser(Login);
            if (checker == null)
            {
                query.AddUser(new User
                {
                    Login = Login,
                    Password = Password,
                    IsAdmin = false
                });
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
