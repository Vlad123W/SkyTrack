using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal class DefaultAuth : Authorization
    {
        public DefaultAuth(string login, string password) : base(login, password)
        {
        }
        public override bool LogIn()
        {
            SqlQuery query = new SqlQuery("skytrack");
            User user = new() { Login = Login, Password = Password, IsAdmin = false };

            if (query.AddUser(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
