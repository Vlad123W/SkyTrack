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
            var checker = query.GetUser(user!.Login!);
            if (checker == null || checker != null && checker.Id == 0)
            {
                query.AddUser(user);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
