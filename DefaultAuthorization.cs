using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal class DefaultAuth(string login, string password) : Authorization(login, password)
    {
        public override bool LogIn()
        {
            SqlQuery query = new("skytrack");

            var checker = query.GetUser(user.Login);
            if (checker != null && checker!.Password == user.Password)
            {
                user = checker;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
