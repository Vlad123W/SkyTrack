﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal class User
    {
        public int Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
