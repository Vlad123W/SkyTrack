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
            SqlQuery query = new("skytrack");

            var checker = query.GetUser(Login);

            if (query.GetUser(Login) != null && checker!.Password == Password)
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
