using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTests
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }
    }
}
