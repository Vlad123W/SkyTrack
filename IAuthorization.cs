using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal interface IAuthorization
    {
        bool Login(string username, string password);
    }
}
